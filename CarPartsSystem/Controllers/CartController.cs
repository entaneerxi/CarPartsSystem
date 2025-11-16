using CarPartsSystem.Data;
using CarPartsSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarPartsSystem.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _context.CartItems
                .Include(ci => ci.Part)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int partId, int quantity = 1)
        {
            var userId = _userManager.GetUserId(User);
            var part = await _context.Parts.FindAsync(partId);

            if (part == null || !part.IsActive || part.StockQuantity < quantity)
            {
                return Json(new { success = false, message = "Part not available" });
            }

            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.PartId == partId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
                _context.Update(existingCartItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId!,
                    PartId = partId,
                    Quantity = quantity,
                    AddedAt = DateTime.Now
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Item added to cart" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var userId = _userManager.GetUserId(User);
            var cartItem = await _context.CartItems
                .Include(ci => ci.Part)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.UserId == userId);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Cart item not found" });
            }

            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else if (cartItem.Part.StockQuantity >= quantity)
            {
                cartItem.Quantity = quantity;
                _context.Update(cartItem);
            }
            else
            {
                return Json(new { success = false, message = "Insufficient stock" });
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = _userManager.GetUserId(User);
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.UserId == userId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            var userId = _userManager.GetUserId(User);
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
