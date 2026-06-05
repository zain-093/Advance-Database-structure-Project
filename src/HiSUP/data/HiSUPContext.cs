using Microsoft.EntityFrameworkCore;
using HiSUP.Models;

namespace HiSUP.Data
{
    public class HiSUPContext : DbContext
    {
        public HiSUPContext(DbContextOptions<HiSUPContext> options) : base(options) { }

        public DbSet<Department> Departments { get; set; }
        public DbSet<AcademicProgram> Programs { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Faculty> Faculty { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<FeeStructure> FeeStructures { get; set; }
        public DbSet<FeePayment> FeePayments { get; set; }
        public DbSet<LibraryItem> LibraryItems { get; set; }
        public DbSet<LibraryIssue> LibraryIssues { get; set; }
        public DbSet<Hostel> Hostels { get; set; }
        public DbSet<HostelAllotment> HostelAllotments { get; set; }
        public DbSet<ExamSchedule> ExamSchedules { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<StudentDocument> StudentDocuments { get; set; }

        public DbSet<StudentDashboardView> StudentDashboard { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite Key for CoursePrerequisite
            modelBuilder.Entity<CoursePrerequisite>()
                .HasKey(cp => new { cp.CourseID, cp.PrerequisiteCourseID });

            // Keyless view mapping
            modelBuilder.Entity<StudentDashboardView>()
                .HasNoKey()
                .ToView("vw_StudentDashboard");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=HITECUNI_DB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
    }
}