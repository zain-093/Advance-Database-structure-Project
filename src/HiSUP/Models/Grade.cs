using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("Grades")]
    public class Grade
    {
        [Key]
        public int GradeID { get; set; }

        public int? EnrollmentID { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? Marks { get; set; }

        [StringLength(2)]
        public string LetterGrade { get; set; }

        [ForeignKey("EnrollmentID")]
        public virtual Enrollment Enrollment { get; set; }
    }
}
