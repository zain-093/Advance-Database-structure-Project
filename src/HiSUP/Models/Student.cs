using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("Students")]
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        [Required]
        [StringLength(20)]
        public string RegistrationNo { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        public int ProgramID { get; set; }

        [ForeignKey("ProgramID")]
        public virtual AcademicProgram Program { get; set; }
    }
}