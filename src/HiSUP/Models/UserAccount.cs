using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("UserAccounts")]
    public class UserAccount
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [StringLength(20)]
        [Column("Role")]
        public string RoleName { get; set; }

        public int ReferenceID { get; set; }
    }
}
