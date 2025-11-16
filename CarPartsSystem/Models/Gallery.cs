using System.ComponentModel.DataAnnotations;

namespace CarPartsSystem.Models
{
    public class Gallery
    {
        [Key]
        public int GalleryId { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public string ImagePath { get; set; } = string.Empty;
        
        public int DisplayOrder { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
