using Microsoft.EntityFrameworkCore;
using CavendishACMISPortal.Models;

namespace CavendishACMISPortal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }

        // ✅ Fixed ambiguous reference
        public DbSet<Module> Modules { get; set; }

        public DbSet<StudentModuleRegistration> Registrations { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<GeneratedPRN> GeneratedPRNs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}