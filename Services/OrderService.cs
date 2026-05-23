using BuildStore.Data;
using BuildStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildStore.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext
            _context;

        public OrderService(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>>
            GetUserOrdersAsync(
                int userId)
        {
            return await _context.Orders

                .Include(o => o.Items)

                .ThenInclude(i => i.Product)

                .Where(o =>
                    o.UserId == userId)

                .OrderByDescending(o =>
                    o.CreatedAt)

                .ToListAsync();
        }

        public async Task CheckoutAsync(
            int userId)
        {
            Cart? cart =
                await _context.Carts

                .Include(c =>
                    c.CartItems)

                .ThenInclude(i =>
                    i.Product)

                .FirstOrDefaultAsync(c =>
                    c.UserId == userId);

            if (cart == null ||
                !cart.CartItems.Any())
            {
                return;
            }

            Order order = new Order
            {
                UserId = userId,

                CreatedAt =
                    DateTime.UtcNow,

                Status =
                    OrderStatus.Processing,

                TotalPrice =
                    cart.CartItems.Sum(i =>
                        i.Quantity *
                        i.Product.Price)
            };

            _context.Orders.Add(order);

            await _context.SaveChangesAsync();

            foreach (CartItem item
                in cart.CartItems)
            {
                OrderItem orderItem =
                    new OrderItem
                    {
                        OrderId = order.Id,

                        ProductId =
                            item.ProductId,

                        Quantity =
                            item.Quantity,

                        Price =
                            item.Product.Price
                    };

                item.Product
                    .QuantityInStock -=
                        item.Quantity;

                _context.OrderItems
                    .Add(orderItem);
            }

            _context.CartItems
                .RemoveRange(
                    cart.CartItems);

            await _context
                .SaveChangesAsync();
        }
    }
}