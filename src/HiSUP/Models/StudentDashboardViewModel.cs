using System;
using System.Collections.Generic;

namespace HiSUP.Models
{
    public class StudentDashboardViewModel
    {
        public Student Student { get; set; }
        public string ProgramName { get; set; }
        public string DepartmentName { get; set; }
        
        // Stats
        public decimal CGPA { get; set; }
        public decimal OutstandingFee { get; set; }
        public decimal AttendancePercentage { get; set; }
        public int EnrolledCoursesCount { get; set; }
        
        // Hostel Allotment
        public string HostelName { get; set; }
        public string RoomNo { get; set; }
        
        // Lists
        public List<EnrollmentDetailsViewModel> Enrollments { get; set; } = new();
        public List<FeePayment> FeePayments { get; set; } = new();
        public List<LibraryIssueDetailsViewModel> LibraryIssues { get; set; } = new();
    }

    public class EnrollmentDetailsViewModel
    {
        public int EnrollmentID { get; set; }
        public string CourseCode { get; set; }
        public string CourseTitle { get; set; }
        public int CreditHours { get; set; }
        public string FacultyName { get; set; }
        public string Semester { get; set; }
        public decimal? Marks { get; set; }
        public string LetterGrade { get; set; }
    }

    public class LibraryIssueDetailsViewModel
    {
        public int IssueID { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status => ReturnDate.HasValue ? "Returned" : "Borrowed";
    }
}
