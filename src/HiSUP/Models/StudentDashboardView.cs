using System.ComponentModel.DataAnnotations;

namespace HiSUP.Models
{
    public class StudentDashboardView
    {
        public int StudentID { get; set; }
        public string RegistrationNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int ProgramID { get; set; }
    }
}