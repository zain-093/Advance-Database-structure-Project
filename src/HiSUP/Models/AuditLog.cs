using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("AuditLog")]
    public class AuditLog
    {
        [Key]
        public int LogID { get; set; }

        [StringLength(100)]
        public string TableName { get; set; }

        [StringLength(20)]
        public string ActionType { get; set; }

        public DateTime? ActionDate { get; set; }
    }
}
