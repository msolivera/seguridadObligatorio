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
    public class LoginLockoutsController : ControllerBase
    {
        private readonly SeguridadContext _context;

        public LoginLockoutsController(SeguridadContext context)
        {
            _context = context;
        }

        // GET: api/LoginLockouts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoginLockout>>> GetLoginLockout()
        {
            return await _context.LoginLockout.ToListAsync();
        }

        // GET: api/LoginLockouts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoginLockout>> GetLoginLockout(string id)
        {
            var loginLockout = await _context.LoginLockout.FindAsync(id);

            if (loginLockout == null)
            {
                return NotFound();
            }

            return loginLockout;
        }

        // PUT: api/LoginLockouts/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLoginLockout(string id, LoginLockout loginLockout)
        {
            if (id != loginLockout.Id)
            {
                return BadRequest();
            }

            _context.Entry(loginLockout).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoginLockoutExists(id))
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

        // POST: api/LoginLockouts
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<LoginLockout>> PostLoginLockout(LoginLockout loginLockout)
        {
            _context.LoginLockout.Add(loginLockout);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LoginLockoutExists(loginLockout.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLoginLockout", new { id = loginLockout.Id }, loginLockout);
        }

        // DELETE: api/LoginLockouts/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<LoginLockout>> DeleteLoginLockout(string id)
        {
            var loginLockout = await _context.LoginLockout.FindAsync(id);
            if (loginLockout == null)
            {
                return NotFound();
            }

            _context.LoginLockout.Remove(loginLockout);
            await _context.SaveChangesAsync();

            return loginLockout;
        }

        private bool LoginLockoutExists(string id)
        {
            return _context.LoginLockout.Any(e => e.Id == id);
        }
    }
}
