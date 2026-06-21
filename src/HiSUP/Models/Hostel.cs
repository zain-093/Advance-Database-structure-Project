using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("Hostels")]
    public class Hostel
    {
        [Key]
        public int HostelID { get; set; }

        [StringLength(100)]
        public string HostelName { get; set; }

        public int? Capacity { get; set; }

        public int? AvailableRooms { get; set; }
    }
}
