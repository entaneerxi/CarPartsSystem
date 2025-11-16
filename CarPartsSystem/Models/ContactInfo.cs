using System.ComponentModel.DataAnnotations;

namespace CarPartsSystem.Models
{
    public class ContactInfo
    {
        [Key]
        public int ContactInfoId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Address { get; set; }
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(100)]
        public string? Email { get; set; }
        
        [StringLength(500)]
        public string? FacebookUrl { get; set; }
        
        [StringLength(500)]
        public string? LineId { get; set; }
        
        [StringLength(500)]
        public string? InstagramUrl { get; set; }
        
        [StringLength(1000)]
        public string? GoogleMapEmbedUrl { get; set; }
        
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
