using Microsoft.EntityFrameworkCore;
using SecureDatabaseApp.Models;

namespace SecureDatabaseApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AppUser> AppUsers { get; set; }
    }
}
