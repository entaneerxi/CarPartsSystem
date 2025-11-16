using CarPartsSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPartsSystem.Models;

namespace CarPartsSystem.ViewComponents
{
    public class CartBadgeViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartBadgeViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (User.Identity?.IsAuthenticated != true || User is not System.Security.Claims.ClaimsPrincipal claimsPrincipal)
            {
                return View(0);
            }

            var userId = _userManager.GetUserId(claimsPrincipal);
            
            if (string.IsNullOrEmpty(userId))
            {
                return View(0);
            }
            
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .ToListAsync();
            
            var cartItemCount = cartItems.Sum(ci => ci.Quantity);

            return View(cartItemCount);
        }
    }
}
