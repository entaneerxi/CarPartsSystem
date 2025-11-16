using CarPartsSystem.Data;
using CarPartsSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarPartsSystem.Controllers
{
    public class PartsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PartsController> _logger;

        public PartsController(ApplicationDbContext context, ILogger<PartsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? category, string? search, string? sortBy)
        {
            var partsQuery = _context.Parts.Where(p => p.IsActive);

            // Filter by category
            if (!string.IsNullOrEmpty(category))
            {
                partsQuery = partsQuery.Where(p => p.Category == category);
                ViewBag.SelectedCategory = category;
            }

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                partsQuery = partsQuery.Where(p => 
                    p.PartName.Contains(search) || 
                    p.Description!.Contains(search) ||
                    p.Brand!.Contains(search));
                ViewBag.SearchTerm = search;
            }

            // Sort
            partsQuery = sortBy switch
            {
                "price_asc" => partsQuery.OrderBy(p => p.Price),
                "price_desc" => partsQuery.OrderByDescending(p => p.Price),
                "name" => partsQuery.OrderBy(p => p.PartName),
                _ => partsQuery.OrderByDescending(p => p.CreatedAt)
            };

            ViewBag.SortBy = sortBy;

            // Get categories for filter dropdown
            ViewBag.Categories = await _context.Parts
                .Where(p => p.IsActive && p.Category != null)
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            var parts = await partsQuery.ToListAsync();
            return View(parts);
        }

        public async Task<IActionResult> Details(int id)
        {
            var part = await _context.Parts
                .FirstOrDefaultAsync(p => p.PartId == id && p.IsActive);

            if (part == null)
            {
                return NotFound();
            }

            return View(part);
        }
    }
}
