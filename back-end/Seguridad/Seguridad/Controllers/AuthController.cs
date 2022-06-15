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
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            //Verifico la existencia del usuario.
            SecurityUser securityUser = _context.SecurityUser.Where(u => u.Username == request.Username).FirstOrDefault();
            //Si existe lo rechazo.
            if (securityUser != null)
            {                
                return BadRequest("User already exists.");
            }
            else
            { 
                //Si no existe lo creo.
                securityUser = new SecurityUser();
                securityUser.Id = Guid.NewGuid().ToString();
                securityUser.Username = request.Username;                
                securityUser.PasswordHash = passwordHash;
                securityUser.PasswordSalt = passwordSalt;
                //Por defecto siempre se crea en el rol de usuario.
                securityUser.RoleId = "6B4F3E3F-5C96-495B-A5EA-0C5289E7C0C4";
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
            //Busco el usuario en la Base de Datos
            SecurityUser securityUser = _context.SecurityUser.Where(u => u.Username == request.Username).FirstOrDefault();
            //Se verifica la existencia
            if (securityUser.Username != request.Username)
            {
                return BadRequest("User not found.");
            }
            //Se verifica la contraseña
            if (!VerifyPasswordHash(request.Password, securityUser.PasswordHash, securityUser.PasswordSalt)) 
            {
                return BadRequest("Wrong password.");
            }
            //Obtengo el rol del usuario.
            Role userRole = _context.Role.Where(u => u.Id == securityUser.RoleId).FirstOrDefault();
            //Creo el JWT
            string token = CreateToken(securityUser, userRole);
            return Ok(token);
        }

        private string CreateToken (SecurityUser user, Role userRole)
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
    }
}
