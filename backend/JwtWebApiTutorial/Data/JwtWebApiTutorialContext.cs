using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using JwtWebApiTutorial.Models;

namespace JwtWebApiTutorial.Data
{
    public class JwtWebApiTutorialContext : DbContext
    {
        public JwtWebApiTutorialContext (DbContextOptions<JwtWebApiTutorialContext> options)
            : base(options)
        {
        }

        public DbSet<JwtWebApiTutorial.Models.User>? User { get; set; }
    }
}
