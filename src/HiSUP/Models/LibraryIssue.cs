using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("LibraryIssues")]
    public class LibraryIssue
    {
        [Key]
        public int IssueID { get; set; }

        public int? StudentID { get; set; }

        public int? ItemID { get; set; }

        [DataType(DataType.Date)]
        public DateTime? IssueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        [ForeignKey("StudentID")]
        public virtual Student Student { get; set; }

        [ForeignKey("ItemID")]
        public virtual LibraryItem LibraryItem { get; set; }
    }
}
