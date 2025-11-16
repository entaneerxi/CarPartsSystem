using CarPartsSystem.Data;
using CarPartsSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarPartsSystem.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class AdminGalleryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminGalleryController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var galleries = await _context.Galleries
                .OrderBy(g => g.DisplayOrder)
                .ThenByDescending(g => g.CreatedAt)
                .ToListAsync();
            return View(galleries);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Gallery gallery, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "gallery");
                    Directory.CreateDirectory(uploadsFolder);
                    
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    
                    gallery.ImagePath = "/uploads/gallery/" + uniqueFileName;
                }

                gallery.CreatedAt = DateTime.Now;
                _context.Add(gallery);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Gallery image added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(gallery);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gallery = await _context.Galleries
                .FirstOrDefaultAsync(m => m.GalleryId == id);
            if (gallery == null)
            {
                return NotFound();
            }

            return View(gallery);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gallery = await _context.Galleries.FindAsync(id);
            if (gallery != null)
            {
                if (!string.IsNullOrEmpty(gallery.ImagePath))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, gallery.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Galleries.Remove(gallery);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Gallery image deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
