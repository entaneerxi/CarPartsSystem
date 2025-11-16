using CarPartsSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarPartsSystem.Controllers
{
    public class PromotionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PromotionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var promotions = await _context.Promotions
                .Where(p => p.IsActive && p.StartDate <= DateTime.Now && p.EndDate >= DateTime.Now)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(promotions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.PromotionId == id && p.IsActive);

            if (promotion == null)
            {
                return NotFound();
            }

            return View(promotion);
        }
    }
}
