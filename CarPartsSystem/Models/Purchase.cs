using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarPartsSystem.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        public string? StaffId { get; set; }
        
        [Required]
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Cancelled, Completed
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        public int? PromotionId { get; set; }
        
        public DateTime? ConfirmedAt { get; set; }
        
        public DateTime? CompletedAt { get; set; }
        
        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;
        
        [ForeignKey("StaffId")]
        public virtual ApplicationUser? Staff { get; set; }
        
        [ForeignKey("PromotionId")]
        public virtual Promotion? Promotion { get; set; }
        
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new List<PurchaseItem>();
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }
}
