using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("ExamSchedule")]
    public class ExamSchedule
    {
        [Key]
        public int ExamID { get; set; }

        public int? CourseID { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExamDate { get; set; }

        [ForeignKey("CourseID")]
        public virtual Course Course { get; set; }
    }
}
