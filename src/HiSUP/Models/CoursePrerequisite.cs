using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("CoursePrerequisites")]
    public class CoursePrerequisite
    {
        public int CourseID { get; set; }
        public int PrerequisiteCourseID { get; set; }

        [ForeignKey("CourseID")]
        public virtual Course Course { get; set; }

        [ForeignKey("PrerequisiteCourseID")]
        public virtual Course PrerequisiteCourse { get; set; }
    }
}
