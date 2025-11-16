using CarPartsSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarPartsSystem.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? status)
        {
            var ordersQuery = _context.Purchases
                .Include(p => p.User)
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Part)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                ordersQuery = ordersQuery.Where(p => p.Status == status);
                ViewBag.Status = status;
            }

            var orders = await ordersQuery
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchase = await _context.Purchases
                .Include(p => p.User)
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Part)
                .Include(p => p.PaymentTransactions)
                .ThenInclude(pt => pt.PaymentMethod)
                .FirstOrDefaultAsync(m => m.PurchaseId == id);

            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
            {
                return NotFound();
            }

            purchase.Status = status;
            if (status == "Confirmed")
            {
                purchase.ConfirmedAt = DateTime.Now;
            }
            else if (status == "Completed")
            {
                purchase.CompletedAt = DateTime.Now;
            }

            _context.Update(purchase);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Order status updated successfully!";
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
