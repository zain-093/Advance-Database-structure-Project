using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("Programs")]
    public class AcademicProgram
    {
        [Key]
        public int ProgramID { get; set; }

        [Required]
        [StringLength(100)]
        public string ProgramName { get; set; }

        public int DepartmentID { get; set; }

        [ForeignKey("DepartmentID")]
        public virtual Department Department { get; set; }
    }
}
