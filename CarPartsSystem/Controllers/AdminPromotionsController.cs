using CarPartsSystem.Data;
using CarPartsSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarPartsSystem.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class AdminPromotionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminPromotionsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var promotions = await _context.Promotions
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(promotions);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Promotion promotion, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "promotions");
                    Directory.CreateDirectory(uploadsFolder);
                    
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    
                    promotion.ImagePath = "/uploads/promotions/" + uniqueFileName;
                }

                promotion.CreatedAt = DateTime.Now;
                _context.Add(promotion);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Promotion created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(promotion);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound();
            }
            return View(promotion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Promotion promotion, IFormFile? imageFile)
        {
            if (id != promotion.PromotionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "promotions");
                        Directory.CreateDirectory(uploadsFolder);
                        
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        
                        if (!string.IsNullOrEmpty(promotion.ImagePath))
                        {
                            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, promotion.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                        
                        promotion.ImagePath = "/uploads/promotions/" + uniqueFileName;
                    }

                    _context.Update(promotion);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Promotion updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PromotionExists(promotion.PromotionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(promotion);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(m => m.PromotionId == id);
            if (promotion == null)
            {
                return NotFound();
            }

            return View(promotion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion != null)
            {
                if (!string.IsNullOrEmpty(promotion.ImagePath))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, promotion.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Promotion deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PromotionExists(int id)
        {
            return _context.Promotions.Any(e => e.PromotionId == id);
        }
    }
}
