using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("Staff")]
    public class Staff
    {
        [Key]
        public int StaffID { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; }

        public int? DepartmentID { get; set; }

        [ForeignKey("DepartmentID")]
        public virtual Department? Department { get; set; }

        public DateTime? HireDate { get; set; }

        [NotMapped]
        public string StaffName
        {
            get => $"{FirstName} {LastName}".Trim();
            set
            {
                var parts = value?.Split(' ', 2);
                if (parts != null && parts.Length > 0)
                {
                    FirstName = parts[0];
                    LastName = parts.Length > 1 ? parts[1] : "";
                }
            }
        }
    }
}
