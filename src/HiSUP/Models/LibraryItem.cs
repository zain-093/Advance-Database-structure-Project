using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("LibraryItems")]
    public class LibraryItem
    {
        [Key]
        public int ItemID { get; set; }

        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(100)]
        public string Author { get; set; }

        public int? CopiesAvailable { get; set; }
    }
}
