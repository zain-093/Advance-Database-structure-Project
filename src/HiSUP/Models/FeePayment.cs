using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiSUP.Models
{
    [Table("FeePayments")]
    public class FeePayment
    {
        [Key]
        public int PaymentID { get; set; }

        public int? StudentID { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? AmountPaid { get; set; }

        public DateTime? PaymentDate { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; }

        public int FeeID { get; set; }

        [ForeignKey("FeeID")]
        public virtual FeeStructure? FeeStructure { get; set; }

        [ForeignKey("StudentID")]
        public virtual Student? Student { get; set; }
    }
}
