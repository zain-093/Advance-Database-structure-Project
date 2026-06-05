using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("Sections")]
    public class Section
    {
        [Key]
        public int SectionID { get; set; }

        public int? CourseID { get; set; }

        public int? FacultyID { get; set; }

        [StringLength(20)]
        public string Semester { get; set; }

        public int? Capacity { get; set; }

        [ForeignKey("CourseID")]
        public virtual Course Course { get; set; }

        [ForeignKey("FacultyID")]
        public virtual Faculty Faculty { get; set; }
    }
}