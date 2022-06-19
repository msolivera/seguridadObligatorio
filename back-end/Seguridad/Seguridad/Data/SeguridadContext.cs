using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Seguridad.Models;

namespace Seguridad.Data
{
    public class SeguridadContext : DbContext
    {
        public SeguridadContext (DbContextOptions<SeguridadContext> options)
            : base(options)
        {
        }

        public DbSet<Seguridad.Models.User> User { get; set; }

        public DbSet<Seguridad.Models.SecurityUser> SecurityUser { get; set; }

        public DbSet<Seguridad.Models.Role> Role { get; set; }

        public DbSet<Seguridad.Models.LoginLockout> LoginLockout { get; set; }
    }
}
