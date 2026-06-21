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

        public int SectionID { get; set; }

        [ForeignKey("SectionID")]
        public virtual Section? Section { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExamDate { get; set; }

        public TimeSpan StartTime { get; set; }
        
        public TimeSpan EndTime { get; set; }

        [Required]
        [StringLength(50)]
        public string Venue { get; set; }

        [ForeignKey("CourseID")]
        public virtual Course? Course { get; set; }
    }
}
