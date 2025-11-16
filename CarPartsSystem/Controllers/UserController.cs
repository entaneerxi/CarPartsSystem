using CarPartsSystem.Data;
using CarPartsSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarPartsSystem.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<UserController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null)
            {
                return NotFound();
            }

            // Get user statistics
            var totalOrders = await _context.Purchases
                .Where(p => p.UserId == userId)
                .CountAsync();

            var totalSpent = await _context.Purchases
                .Where(p => p.UserId == userId && p.Status == "Completed")
                .SumAsync(p => (decimal?)p.TotalAmount) ?? 0;

            var pendingOrders = await _context.Purchases
                .Where(p => p.UserId == userId && p.Status == "Pending")
                .CountAsync();

            var recentOrders = await _context.Purchases
                .Where(p => p.UserId == userId)
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Part)
                .OrderByDescending(p => p.PurchaseDate)
                .Take(5)
                .ToListAsync();

            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalSpent = totalSpent;
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.RecentOrders = recentOrders;
            ViewBag.User = user;

            return View();
        }

        public async Task<IActionResult> Profile()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId!);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> PurchaseHistory(string? status)
        {
            var userId = _userManager.GetUserId(User);

            var purchasesQuery = _context.Purchases
                .Where(p => p.UserId == userId)
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Part)
                .Include(p => p.PaymentTransactions)
                .ThenInclude(pt => pt.PaymentMethod)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                purchasesQuery = purchasesQuery.Where(p => p.Status == status);
                ViewBag.StatusFilter = status;
            }

            var purchases = await purchasesQuery
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            return View(purchases);
        }
    }
}
