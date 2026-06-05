namespace HiSUP.Models
{
    public class StudentDashboardView
    {
        public int StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Semester { get; set; }
        public string CourseName { get; set; }
        public int TotalSemesterCourses { get; set; }
        public decimal RunningTotalFeesPaid { get; set; }
    }
}