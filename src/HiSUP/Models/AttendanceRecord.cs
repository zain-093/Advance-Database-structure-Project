using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("AttendanceRecords")]
    public class AttendanceRecord
    {
        [Key]
        public int AttendanceID { get; set; }

        public int? StudentID { get; set; }

        public int? SectionID { get; set; }

        public int? EnrollmentID { get; set; }

        [ForeignKey("EnrollmentID")]
        public virtual Enrollment? Enrollment { get; set; }

        [DataType(DataType.Date)]
        [Column("ClassDate")]
        public DateTime? AttendanceDate { get; set; }

        [StringLength(10)]
        public string Status { get; set; }

        [ForeignKey("StudentID")]
        public virtual Student? Student { get; set; }

        [ForeignKey("SectionID")]
        public virtual Section? Section { get; set; }
    }
}
