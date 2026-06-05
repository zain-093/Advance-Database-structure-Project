using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("StudentDocuments")]
    public class StudentDocument
    {
        [Key]
        public int DocumentID { get; set; }

        public int? StudentID { get; set; }

        [StringLength(200)]
        public string DocumentName { get; set; }

        [ForeignKey("StudentID")]
        public virtual Student Student { get; set; }
    }
}
