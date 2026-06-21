using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("Results")]
    public class Result
    {
        [Key]
        public int ResultID { get; set; }

        public int? StudentID { get; set; }

        [StringLength(20)]
        public string Semester { get; set; }

        [Column("SGPA", TypeName = "decimal(3,2)")]
        public decimal? GPA { get; set; }

        [Column(TypeName = "decimal(3,2)")]
        public decimal? CGPA { get; set; }

        [ForeignKey("StudentID")]
        public virtual Student? Student { get; set; }
    }
}
