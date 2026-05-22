using BuildStore.Data;
using BuildStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace BuildStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult MyOrders()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            string? userIdString =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            int userId = int.Parse(userIdString);

            List<Order> orders = _context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)

                .Where(o => o.UserId == userId)

                .OrderByDescending(o => o.CreatedAt)

                .ToList();

            return View(orders);
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            Console.WriteLine("CHECKOUT STARTED");//11111111111111111111111111111111111
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            string? userIdString =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            int userId = int.Parse(userIdString);

            Cart? cart = _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(c =>
                    c.UserId == userId);

            if (cart == null ||
                !cart.CartItems.Any())
            {
                return RedirectToAction(
                    "Index",
                    "Cart");
            }

            Order order = new Order
            {
                UserId = userId,

                CreatedAt = DateTime.UtcNow,

                Status = "Processing",

                TotalPrice = cart.CartItems.Sum(i =>
                    i.Quantity * i.Product.Price)
            };

            _context.Orders.Add(order);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);//111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111
            }

            foreach (CartItem item in cart.CartItems)
            {
                OrderItem orderItem =
                    new OrderItem
                    {
                        OrderId = order.Id,

                        ProductId = item.ProductId,

                        Quantity = item.Quantity,

                        Price = item.Product.Price
                    };

                item.Product.QuantityInStock -=
                    item.Quantity;

                _context.OrderItems.Add(orderItem);
            }

            _context.CartItems.RemoveRange(
                cart.CartItems);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }//111111111111111111111111111111111111111111111111111111

            return RedirectToAction(
                "MyOrders");
        }
    }
}