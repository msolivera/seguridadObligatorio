using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seguridad.Data;
using Seguridad.Models;

namespace Seguridad.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly SeguridadContext _context;

        public LoginController(SeguridadContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(User request)
        {
            User usu = _context.User.Where(u => u.Mail == request.Mail).FirstOrDefault();
            if (usu != null)
            {

                //if (user.Username != request.Username)
                //{
                //    return BadRequest("User not found.");
                //}

                //if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                //{
                //    return BadRequest("Wrong password.");
                //}

                //string token = CreateToken(user);

                //var refreshToken = GenerateRefreshToken();
                //SetRefreshToken(refreshToken);

                //return Ok(token);
            }
        }
    }
}
