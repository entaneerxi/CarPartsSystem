using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarPartsSystem.Models
{
    public class PurchaseItem
    {
        [Key]
        public int PurchaseItemId { get; set; }
        
        [Required]
        public int PurchaseId { get; set; }
        
        [Required]
        public int PartId { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
        
        // Navigation properties
        [ForeignKey("PurchaseId")]
        public virtual Purchase Purchase { get; set; } = null!;
        
        [ForeignKey("PartId")]
        public virtual Part Part { get; set; } = null!;
    }
}
