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
        [Column("Action")]
        public string ActionType { get; set; }

        [Column("LogTimestamp")]
        public DateTime? ActionDate { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        [StringLength(100)]
        public string? DatabaseUser { get; set; }
    }
}
