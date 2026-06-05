using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("FeeStructure")]
    public class FeeStructure
    {
        [Key]
        public int FeeID { get; set; }

        public int? ProgramID { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Amount { get; set; }

        [ForeignKey("ProgramID")]
        public virtual AcademicProgram Program { get; set; }
    }
}
