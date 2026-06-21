using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("Departments")]
    public class Department
    {
        [Key]
        public int DepartmentID { get; set; }

        [Required]
        [StringLength(100)]
        [Column("DeptName")]
        public string DepartmentName { get; set; }

        [Required]
        [StringLength(10)]
        [Column("DeptCode")]
        public string DepartmentCode { get; set; }

        public int? EstablishedYear { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
