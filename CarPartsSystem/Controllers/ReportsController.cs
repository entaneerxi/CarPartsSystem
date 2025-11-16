using CarPartsSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarPartsSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SalesReport(DateTime? startDate, DateTime? endDate, string reportType = "daily")
        {
            if (!startDate.HasValue)
            {
                startDate = DateTime.Now.Date;
            }
            if (!endDate.HasValue)
            {
                endDate = DateTime.Now.Date;
            }

            var purchases = await _context.Purchases
                .Include(p => p.User)
                .Where(p => p.PurchaseDate >= startDate && p.PurchaseDate <= endDate.Value.AddDays(1))
                .Where(p => p.Status == "Completed")
                .OrderByDescending(p => p.PurchaseDate)
                .ToListAsync();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.ReportType = reportType;
            ViewBag.TotalSales = purchases.Sum(p => p.TotalAmount);
            ViewBag.TotalOrders = purchases.Count;

            return View(purchases);
        }

        public async Task<IActionResult> StaffSalesReport(DateTime? startDate, DateTime? endDate)
        {
            if (!startDate.HasValue)
            {
                startDate = DateTime.Now.AddMonths(-1).Date;
            }
            if (!endDate.HasValue)
            {
                endDate = DateTime.Now.Date;
            }

            var staffSales = await _context.Purchases
                .Include(p => p.Staff)
                .Where(p => p.PurchaseDate >= startDate && p.PurchaseDate <= endDate.Value.AddDays(1))
                .Where(p => p.Status == "Completed" && p.StaffId != null)
                .GroupBy(p => new { p.StaffId, p.Staff!.Email, p.Staff.FirstName, p.Staff.LastName })
                .Select(g => new
                {
                    StaffId = g.Key.StaffId,
                    Email = g.Key.Email,
                    FirstName = g.Key.FirstName,
                    LastName = g.Key.LastName,
                    TotalSales = g.Sum(p => p.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderByDescending(s => s.TotalSales)
                .ToListAsync();

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            return View(staffSales);
        }
    }
}
