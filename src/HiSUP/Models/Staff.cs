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
        [StringLength(100)]
        public string StaffName { get; set; }
    }
}
