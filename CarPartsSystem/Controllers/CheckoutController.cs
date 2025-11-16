using CarPartsSystem.Data;
using CarPartsSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarPartsSystem.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<CheckoutController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            
            // Get cart items
            var cartItems = await _context.CartItems
                .Include(ci => ci.Part)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Message"] = "Your cart is empty. Please add items before checkout.";
                return RedirectToAction("Index", "Cart");
            }

            // Check stock availability
            foreach (var item in cartItems)
            {
                if (!item.Part.IsActive || item.Part.StockQuantity < item.Quantity)
                {
                    TempData["Error"] = $"Part '{item.Part.PartName}' is not available in the requested quantity.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            // Get active payment methods
            var paymentMethods = await _context.PaymentMethods
                .Where(pm => pm.IsActive)
                .ToListAsync();

            ViewBag.PaymentMethods = paymentMethods;
            ViewBag.CartItems = cartItems;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCheckout(int paymentMethodId, string? notes)
        {
            var userId = _userManager.GetUserId(User);
            
            // Get cart items
            var cartItems = await _context.CartItems
                .Include(ci => ci.Part)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // Validate stock availability
            foreach (var item in cartItems)
            {
                if (!item.Part.IsActive || item.Part.StockQuantity < item.Quantity)
                {
                    TempData["Error"] = $"Part '{item.Part.PartName}' is not available in the requested quantity.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            // Validate payment method
            var paymentMethod = await _context.PaymentMethods.FindAsync(paymentMethodId);
            if (paymentMethod == null || !paymentMethod.IsActive)
            {
                TempData["Error"] = "Invalid payment method selected.";
                return RedirectToAction(nameof(Index));
            }

            // Calculate totals
            decimal subtotal = cartItems.Sum(item => item.Part.Price * item.Quantity);
            decimal taxAmount = subtotal * 0.07m; // 7% tax
            decimal totalAmount = subtotal + taxAmount;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create Purchase
                var purchase = new Purchase
                {
                    UserId = userId!,
                    PurchaseDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    DiscountAmount = 0,
                    Status = "Pending",
                    Notes = notes
                };
                _context.Purchases.Add(purchase);
                await _context.SaveChangesAsync();

                // Create Purchase Items and update stock
                foreach (var cartItem in cartItems)
                {
                    var purchaseItem = new PurchaseItem
                    {
                        PurchaseId = purchase.PurchaseId,
                        PartId = cartItem.PartId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Part.Price,
                        Subtotal = cartItem.Part.Price * cartItem.Quantity
                    };
                    _context.PurchaseItems.Add(purchaseItem);

                    // Update stock quantity
                    var part = await _context.Parts.FindAsync(cartItem.PartId);
                    if (part != null)
                    {
                        part.StockQuantity -= cartItem.Quantity;
                        _context.Update(part);
                    }
                }

                // Create Payment Transaction
                var paymentTransaction = new PaymentTransaction
                {
                    PurchaseId = purchase.PurchaseId,
                    PaymentMethodId = paymentMethodId,
                    UserId = userId!,
                    Amount = totalAmount,
                    TransactionDate = DateTime.Now,
                    Status = "Pending",
                    TransactionReference = $"TXN-{purchase.PurchaseId}-{DateTime.Now:yyyyMMddHHmmss}"
                };
                _context.PaymentTransactions.Add(paymentTransaction);

                // Clear cart
                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Your order has been placed successfully!";
                return RedirectToAction(nameof(Confirmation), new { id = purchase.PurchaseId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error processing checkout");
                TempData["Error"] = "An error occurred while processing your order. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var userId = _userManager.GetUserId(User);
            
            var purchase = await _context.Purchases
                .Include(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Part)
                .Include(p => p.PaymentTransactions)
                .ThenInclude(pt => pt.PaymentMethod)
                .FirstOrDefaultAsync(p => p.PurchaseId == id && p.UserId == userId);

            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }
    }
}
