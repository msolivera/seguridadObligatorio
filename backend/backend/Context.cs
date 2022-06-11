using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using backend.Models;

namespace backend
{
    public class Context : DbContext
    {
        public Context() : base("obligatorioSeguridad")
        {

        }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}
