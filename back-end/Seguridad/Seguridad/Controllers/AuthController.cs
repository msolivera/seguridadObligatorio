using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Seguridad.Data;
using Seguridad.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Seguridad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static SecurityUser user = new SecurityUser();
        private readonly IConfiguration _configuration;
        private readonly SeguridadContext _context;

        public AuthController(IConfiguration configuration, SeguridadContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<SecurityUser>> Register(UserDto request)
        {
            //Se valida formato de correo en el Username.            
            if (!ValidateUserName(request.Username))
            {
                return BadRequest("Invalid Username format.");
            }
            //Se valida formato de contraseña.            
            if (!ValidateUserPassword(request.Password))
            {
                return BadRequest("Invalid Password format.");
            }
            //Se crea el PasswordHash
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            //Verifico la existencia del usuario.
            SecurityUser securityUser = _context.SecurityUser.Where(u => u.Username == request.Username).FirstOrDefault();
            //Si existe se rechaza.
            if (securityUser != null)
            {
                return BadRequest("User already exists.");
            }
            else
            {
                //Si no existe se crea.
                securityUser = new SecurityUser();
                securityUser.Id = Guid.NewGuid().ToString();
                securityUser.Username = request.Username;
                securityUser.PasswordHash = passwordHash;
                securityUser.PasswordSalt = passwordSalt;
                //Por defecto siempre se crea en el rol de invitado.
                securityUser.RoleId = "918D46B1-5BCB-47DF-8F9B-A9B68740E730";
                _context.SecurityUser.Add(securityUser);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    throw;
                }
                return Ok(securityUser);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            //Se valida el formato de correo en el Username.            
            if (!ValidateUserName(request.Username))
            {
                return BadRequest("Invalid Username format.");
            }
            //Se valida el formato de contraseña.            
            if (!ValidateUserPassword(request.Password))
            {
                return BadRequest("Invalid Password format.");
            }
            //Se verifica que el usuario no esté bloqueado
            string verifyLockedPassword = VerifiyLockedPassword(request.Username);
            if (verifyLockedPassword != null)
            {
                return BadRequest(verifyLockedPassword);
            }
            //Se busca el usuario en la Base de Datos.
            SecurityUser securityUser = _context.SecurityUser.Where(u => u.Username == request.Username).FirstOrDefault();
            //Se verifica existencia de usuario.
            if (securityUser == null)
            {
                return BadRequest("User not found.");
            }
            //Se verifica la contraseña
            if (!VerifyPasswordHash(request.Password, securityUser.PasswordHash, securityUser.PasswordSalt))
            {
                return LockPassword(request.Username).Result;
            }
            //Se obtiene el rol del usuario.
            Role userRole = _context.Role.Where(u => u.Id == securityUser.RoleId).FirstOrDefault();
            if (userRole == null)
            {
                return BadRequest("The user does not have a role.");
            }
            //Creo el JWT
            string token = CreateToken(securityUser, userRole);
            return Ok(token);
        }

        private string CreateToken(SecurityUser user, Role userRole)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("Name", user.Username),
                new Claim("Role", userRole.Name)
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private bool ValidateUserName(string username)
        {
            var userNameRegex = new Regex(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$");
            return userNameRegex.IsMatch(username);
        }
        private bool ValidateUserPassword(string password)
        {
            var userPassRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])([A-Za-z\d$@$!%*?&]|[^ ]){8,20}$");
            return userPassRegex.IsMatch(password);
        }
        private string VerifiyLockedPassword(string username)
        {
            string result = null;
            DateTime actualTime = DateTime.Now;            
            List<LoginLockout> selectedAttempts = PreviousAttempts(username);
            int totalAttempts = selectedAttempts.Count;
            if (totalAttempts >= 3)
            {
                DateTime deadLine = (DateTime)selectedAttempts[2].ExpirationTime;
                if (actualTime > deadLine)
                {
                    return result;
                }
                else
                {
                    //Se envía un Bad Request diciendo que la cuenta se encuentra bloqueada por x tiempo.
                    string remainingTime = Math.Round(((deadLine - actualTime).TotalMinutes), 2).ToString();
                    result = remainingTime;
                    return result;
                }                    
            }
            else
            {
                return result;
            }
        }
        private async Task<ActionResult<string>> LockPassword(string username)
        {
            DateTime actualTime = DateTime.Now;
            List<LoginLockout> previousAttempts = await _context.LoginLockout.Where(u => u.Username == username).ToListAsync();
            List<LoginLockout> validAttempts = new List<LoginLockout>();
            foreach (LoginLockout attempt in previousAttempts)
            {
                DateTime deadLine = (DateTime)attempt.ExpirationTime;
                if (actualTime > deadLine)
                {
                    _context.LoginLockout.Remove(attempt);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    validAttempts.Add(attempt);
                }

            }
            List<LoginLockout> selectedAttempts = validAttempts.OrderBy(o => o.ExpirationTime).ToList();
            int totalAttempts = selectedAttempts.Count;
            if (totalAttempts < 3)
            {
                LoginLockout attempt = new LoginLockout();
                attempt.Id = Guid.NewGuid().ToString();
                attempt.Username = username;
                attempt.ExpirationTime = DateTime.Now.AddMinutes(1);
                _context.LoginLockout.Add(attempt);
                await _context.SaveChangesAsync();
                return BadRequest("Wrong password.");
            }
            else
            {
                DateTime deadLine = (DateTime)selectedAttempts[2].ExpirationTime;
                if (actualTime > deadLine)
                {
                    foreach (LoginLockout attempt in selectedAttempts)
                    {
                        _context.LoginLockout.Remove(attempt);
                        await _context.SaveChangesAsync();
                    }
                    return BadRequest("Unlocked password, try again.");
                }
                else
                {
                    //Se envía un Bad Request diciendo que la cuenta se encuentra bloqueada por x tiempo.
                    string remainingTime = Math.Round(((deadLine - actualTime).TotalMinutes), 2).ToString();
                    return BadRequest("Password locked for: " + remainingTime + " minutes.");
                }
            }
        }
        private List<LoginLockout> PreviousAttempts(string username)
        {
            DateTime actualTime = DateTime.Now;
            var previousAttempts = _context.LoginLockout.Where(u => u.Username == username);
            List<LoginLockout> validAttempts = new List<LoginLockout>();
            foreach (LoginLockout attempt in previousAttempts)
            {               
                validAttempts.Add(attempt);                
            }
            List<LoginLockout> selectedAttempts = validAttempts.OrderBy(o => o.ExpirationTime).ToList();
            return selectedAttempts;
        }
    }
}
