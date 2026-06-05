using Microsoft.EntityFrameworkCore;
using HiSUP.Models;

namespace HiSUP.Data
{
    public class HiSUPContext : DbContext
    {
        public HiSUPContext(DbContextOptions<HiSUPContext> options) : base(options) { }

        // Core Models
        public DbSet<Student> Students { get; set; }
        public DbSet<Faculty> Faculty { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Section> Sections { get; set; } 
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Primary Keys if EF Core doesn't automatically detect them
            modelBuilder.Entity<Student>().HasKey(s => s.StudentID);
            modelBuilder.Entity<Faculty>().HasKey(f => f.FacultyID);
            modelBuilder.Entity<Course>().HasKey(c => c.CourseID);
            modelBuilder.Entity<Enrollment>().HasKey(e => e.EnrollmentID);
            modelBuilder.Entity<Section>().HasKey(s => s.SectionID);
        }
    }
}