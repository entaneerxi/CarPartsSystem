using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarPartsSystem.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }
        
        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [Required]
        public int PartId { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        public DateTime AddedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;
        
        [ForeignKey("PartId")]
        public virtual Part Part { get; set; } = null!;
    }
}
