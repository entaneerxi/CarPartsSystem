using System.ComponentModel.DataAnnotations;

namespace CarPartsSystem.Models
{
    public class PaymentMethod
    {
        [Key]
        public int PaymentMethodId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string MethodName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string? IconPath { get; set; }
        
        // Navigation properties
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }
}
