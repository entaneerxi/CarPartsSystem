using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarPartsSystem.Models
{
    public class BookingPostponeRequest
    {
        [Key]
        public int RequestId { get; set; }
        
        [Required]
        public int PurchaseId { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public DateTime RequestedDate { get; set; } = DateTime.Now;
        
        [Required]
        public DateTime NewProposedDate { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Reason { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        
        [StringLength(500)]
        public string? AdminNotes { get; set; }
        
        public string? ReviewedBy { get; set; }
        
        public DateTime? ReviewedAt { get; set; }
        
        // Navigation properties
        [ForeignKey("PurchaseId")]
        public virtual Purchase Purchase { get; set; } = null!;
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
