using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("Courses")]
    public class Course
    {
        [Key]
        public int CourseID { get; set; }

        [Required]
        [StringLength(20)]
        public string CourseCode { get; set; }

        [Required]
        [StringLength(100)]
        [Column("CourseName")]
        public string CourseTitle { get; set; }

        public int? CreditHours { get; set; }

        public int DepartmentID { get; set; }

        [ForeignKey("DepartmentID")]
        public virtual Department? Department { get; set; }
    }
}