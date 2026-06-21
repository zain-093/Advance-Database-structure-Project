using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("HostelAllotments")]
    public class HostelAllotment
    {
        [Key]
        public int AllotmentID { get; set; }

        public int? StudentID { get; set; }

        public int? HostelID { get; set; }

        [StringLength(20)]
        [Column("RoomNumber")]
        public string RoomNo { get; set; }

        public DateTime? AllotmentDate { get; set; }

        [ForeignKey("StudentID")]
        public virtual Student? Student { get; set; }

        [ForeignKey("HostelID")]
        public virtual Hostel? Hostel { get; set; }
    }
}
