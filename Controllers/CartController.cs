using BuildStore.Data;
using BuildStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BuildStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int userId = 1;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            return View(cart);
        }

        public async Task<IActionResult> Add(int productId)
        {
            int userId = 1;

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>()
                };

                _context.Carts.Add(cart);

                await _context.SaveChangesAsync();
            }

            var existingItem = cart.CartItems
                .FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = 1
                });
            }



            await _context.SaveChangesAsync();

            return Redirect(Request.Headers["Referer"].ToString());
        }
        public async Task<IActionResult> Increase(int itemId)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == itemId);

            if (item != null)
            {
                item.Quantity++;

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Decrease(int itemId)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == itemId);

            if (item != null)
            {
                item.Quantity--;

                if (item.Quantity <= 0)
                {
                    _context.CartItems.Remove(item);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Remove(int itemId)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == itemId);

            if (item != null)
            {
                _context.CartItems.Remove(item);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}