using Microsoft.AspNetCore.Identity;

namespace CarPartsSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? ProfileImagePath { get; set; }
        
        // Navigation property
        public virtual ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }
}
