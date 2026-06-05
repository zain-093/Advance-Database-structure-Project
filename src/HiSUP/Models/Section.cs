using System.ComponentModel.DataAnnotations;

namespace HiSUP.Models
{
    public class Section
    {
        public int SectionID { get; set; }
        public int CourseID { get; set; }
        public int FacultyID { get; set; }
        [Required, StringLength(20)] public string Semester { get; set; }
        [Required, StringLength(10)] public string SectionName { get; set; }
        public int AvailableSeats { get; set; }
    }
}