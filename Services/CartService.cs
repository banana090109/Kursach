using BuildStore.Data;
using BuildStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildStore.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartAsync(int userId)
        {
            return
                await _context.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async Task<(bool Success, string Message)> AddAjaxAsync(int userId, int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return (false, "Product not found.");

            if (product.Stock <= 0)
                return (false, "This product is out of stock.");

            // Твоя оригінальна логіка додавання в кошик
            var existingItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

            if (existingItem != null)
                existingItem.Quantity += 1;
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1
                });
            }

            await _context.SaveChangesAsync();
            return (true, "Product added to cart");
        }

        public async Task<int>
        GetCartCountAsync(int userId)
        {
            return 
                await _context.CartItems.Where(ci => ci.Cart.UserId == userId).SumAsync(ci => ci.Quantity);
        }

        public async Task AddToCartAsync(int userId, int productId)
        {
            var cart = await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);

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

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
                existingItem.Quantity++;
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,

                    Quantity = 1
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task IncreaseAsync(int itemId)
        {
            var item = await _context.CartItems.FirstOrDefaultAsync(ci => ci.Id == itemId);

            if (item != null)
            {
                item.Quantity++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DecreaseAsync(
            int itemId)
        {
            var item =
                await _context.CartItems.FirstOrDefaultAsync(ci => ci.Id == itemId);

            if (item != null)
            {
                item.Quantity--;

                if (item.Quantity <= 0)
                    _context.CartItems.Remove(item);

                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAsync(int itemId)
        {
            var item = await _context.CartItems.FirstOrDefaultAsync(ci => ci.Id == itemId);

            if (item != null)
            {
                _context.CartItems.Remove(item);

                await _context.SaveChangesAsync();
            }
        }
    }
}