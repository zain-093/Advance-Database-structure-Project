using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("Enrollments")]
    public class Enrollment
    {
        [Key]
        public int EnrollmentID { get; set; }

        public int? StudentID { get; set; }

        public int? SectionID { get; set; }

        public int? CourseID { get; set; }

        [ForeignKey("CourseID")]
        public virtual Course? Course { get; set; }

        [StringLength(20)]
        public string? Semester { get; set; }

        public DateTime? EnrollmentDate { get; set; }

        [ForeignKey("StudentID")]
        public virtual Student? Student { get; set; }

        [ForeignKey("SectionID")]
        public virtual Section? Section { get; set; }
    }
}