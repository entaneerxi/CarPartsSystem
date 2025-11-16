using CarPartsSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarPartsSystem.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Dashboard statistics
            ViewBag.TotalParts = await _context.Parts.CountAsync();
            ViewBag.TotalOrders = await _context.Purchases.CountAsync();
            ViewBag.PendingOrders = await _context.Purchases.CountAsync(p => p.Status == "Pending");
            ViewBag.TotalRevenue = await _context.Purchases
                .Where(p => p.Status == "Completed")
                .SumAsync(p => p.TotalAmount);
            
            // Recent orders
            var recentOrders = await _context.Purchases
                .Include(p => p.User)
                .OrderByDescending(p => p.PurchaseDate)
                .Take(10)
                .ToListAsync();

            return View(recentOrders);
        }

        public IActionResult ManageParts()
        {
            return RedirectToAction("Index", "AdminParts");
        }

        public IActionResult ManagePromotions()
        {
            return RedirectToAction("Index", "AdminPromotions");
        }

        public IActionResult ManageGallery()
        {
            return RedirectToAction("Index", "AdminGallery");
        }

        public IActionResult ManageOrders()
        {
            return RedirectToAction("Index", "AdminOrders");
        }

        public IActionResult Reports()
        {
            return RedirectToAction("Index", "Reports");
        }
    }
}
