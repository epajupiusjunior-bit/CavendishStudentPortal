using Microsoft.EntityFrameworkCore;
using CavendishACMISPortal.Models;

namespace CavendishACMISPortal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<LecturerModuleAssignment> LecturerAssignments { get; set; }
        public DbSet<StudentModuleRegistration> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraint to prevent duplicate module registrations
            modelBuilder.Entity<StudentModuleRegistration>()
                .HasIndex(r => new { r.UserId, r.ModuleId })
                .IsUnique();
        }
    }
}