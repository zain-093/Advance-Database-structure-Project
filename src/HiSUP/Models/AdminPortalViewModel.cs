using System.Collections.Generic;

namespace HiSUP.Models
{
    public class AdminPortalViewModel
    {
        public List<Department> Departments { get; set; } = new();
        public List<AcademicProgram> Programs { get; set; } = new();
        public List<Student> Students { get; set; } = new();
        public List<Faculty> Faculty { get; set; } = new();
        public List<Staff> Staffs { get; set; } = new();
        public List<Course> Courses { get; set; } = new();
        public List<CoursePrerequisite> CoursePrerequisites { get; set; } = new();
        public List<Section> Sections { get; set; } = new();
        public List<Enrollment> Enrollments { get; set; } = new();
        public List<Grade> Grades { get; set; } = new();
        public List<AttendanceRecord> AttendanceRecords { get; set; } = new();
        public List<FeeStructure> FeeStructures { get; set; } = new();
        public List<FeePayment> FeePayments { get; set; } = new();
        public List<LibraryItem> LibraryItems { get; set; } = new();
        public List<LibraryIssue> LibraryIssues { get; set; } = new();
        public List<Hostel> Hostels { get; set; } = new();
        public List<HostelAllotment> HostelAllotments { get; set; } = new();
        public List<ExamSchedule> ExamSchedules { get; set; } = new();
        public List<Result> Results { get; set; } = new();
        public List<UserAccount> UserAccounts { get; set; } = new();
        public List<AuditLog> AuditLogs { get; set; } = new();
        public List<Notification> Notifications { get; set; } = new();
        public List<StudentDocument> StudentDocuments { get; set; } = new();

        // ===== Admin KPI Summary =====
        public decimal TotalFeeCollected { get; set; }
        public int HostelOccupancy { get; set; }
        public int TotalLibraryBooks { get; set; }

        // ===== Faculty-specific =====
        public string FacultyName { get; set; }
        public int FacultyAssignedCourses { get; set; }
        public int FacultySections { get; set; }
        public int FacultyStudentCount { get; set; }
    }
}
