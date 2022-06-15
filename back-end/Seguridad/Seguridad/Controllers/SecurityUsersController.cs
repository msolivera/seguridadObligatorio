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
    public class SecurityUsersController : ControllerBase
    {
        private readonly SeguridadContext _context;

        public SecurityUsersController(SeguridadContext context)
        {
            _context = context;
        }

        // GET: api/SecurityUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SecurityUser>>> GetSecurityUser()
        {
            return await _context.SecurityUser.ToListAsync();
        }

        // GET: api/SecurityUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SecurityUser>> GetSecurityUser(string id)
        {
            var securityUser = await _context.SecurityUser.FindAsync(id);

            if (securityUser == null)
            {
                return NotFound();
            }

            return securityUser;
        }

        // PUT: api/SecurityUsers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSecurityUser(string id, SecurityUser securityUser)
        {
            if (id != securityUser.Id)
            {
                return BadRequest();
            }

            _context.Entry(securityUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SecurityUserExists(id))
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

        // POST: api/SecurityUsers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<SecurityUser>> PostSecurityUser(SecurityUser securityUser)
        {
            _context.SecurityUser.Add(securityUser);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SecurityUserExists(securityUser.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSecurityUser", new { id = securityUser.Id }, securityUser);
        }

        // DELETE: api/SecurityUsers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<SecurityUser>> DeleteSecurityUser(string id)
        {
            var securityUser = await _context.SecurityUser.FindAsync(id);
            if (securityUser == null)
            {
                return NotFound();
            }

            _context.SecurityUser.Remove(securityUser);
            await _context.SaveChangesAsync();

            return securityUser;
        }

        private bool SecurityUserExists(string id)
        {
            return _context.SecurityUser.Any(e => e.Id == id);
        }
    }
}
