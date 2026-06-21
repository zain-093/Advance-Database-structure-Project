using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("Students")]
    public class Student
    {
        [Key]
        public int StudentID { get; set; }

        [StringLength(20)]
        public string? RegistrationNo { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(15)]
        [Column("CNIC_ClearText")]
        public string? CNIC_ClearText { get; set; }

        public int DepartmentID { get; set; }

        [ForeignKey("DepartmentID")]
        public virtual Department? Department { get; set; }

        public int? ProgramID { get; set; }

        [ForeignKey("ProgramID")]
        public virtual AcademicProgram? Program { get; set; }

        public DateTime? EnrollmentDate { get; set; }
    }
}