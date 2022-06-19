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
    public class UsersController : ControllerBase
    {
        private readonly SeguridadContext _context;

        public UsersController(SeguridadContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            string mail = "usuario@mail.com";
            List<User> systemUsers = await _context.User.ToListAsync();
            List<User> selectedUser = new List<User>();
            foreach(User user in systemUsers)
            {
                if(user.Mail == mail)
                {
                    selectedUser.Add(user);
                }
            }
            List<User> users = selectedUser.OrderBy(o => o.JwtCreationTime).ToList();
            int totalUsers = users.Count;            
            if (totalUsers < 3)
            {
                User user = new User();
                //user.Id = Guid.NewGuid().ToString();
                Random rnd = new Random();
                user.Id = rnd.Next(1,1000);
                user.Mail = "usuario@mail.com";
                user.Password = "Test1234";
                user.JwtCreationTime = DateTime.Now.AddMinutes(1);
                user.JwtExpirationTime = null;
                user.Salt = "1234";
                _context.User.Add(user);
                await _context.SaveChangesAsync();

                //Se avisa que la contraseña es incorrecta
            }
            else
            {
                DateTime ahora = DateTime.Now;
                DateTime deadLine = (DateTime)users[2].JwtCreationTime;
                if (ahora > deadLine)
                {
                    foreach (User user in users)
                    {                        
                        _context.User.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                    //Luego se intenta un login
                }
                else
                {
                    //Se envía un Bad Request diciendo que la cuenta se encuentra bloqueada por x tiempo.
                    string remainingTime = (deadLine - ahora).TotalMinutes.ToString();
                }                
            }
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            user.JwtCreationTime = null;
            user.JwtExpirationTime = null;
            user.Salt = "1234";
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }
    }
}
