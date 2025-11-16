using CarPartsSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarPartsSystem.Controllers
{
    public class GalleryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GalleryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var galleries = await _context.Galleries
                .Where(g => g.IsActive)
                .OrderBy(g => g.DisplayOrder)
                .ThenByDescending(g => g.CreatedAt)
                .ToListAsync();

            return View(galleries);
        }
    }
}
