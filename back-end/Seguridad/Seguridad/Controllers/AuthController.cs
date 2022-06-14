using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            user.Username = request.Username;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            

            User usu = _context.User.Where(u => u.Mail == request.Username).FirstOrDefault();

            if (user.Username != request.Username)
            {
                return BadRequest("User not found.");
            }
            if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt)) 
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(user);
            return Ok(token);
        }

        private string CreateToken (SecurityUser user)
        {
            //Agregar Rol a manopla
            //new Claim(ClaimTypes.Role, user.User.Role)
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
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
