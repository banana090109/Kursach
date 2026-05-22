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

            Console.WriteLine(cart == null);

            Console.WriteLine(cart.CartItems == null);

            Console.WriteLine(cart.CartItems.Count);


            if (cart == null ||
                !cart.CartItems.Any())
            {
                TempData["Error"] = "Cart is empty!";

                return RedirectToAction(
                    "Index",
                    "Cart");
            }

            Order order = new Order
            {
                UserId = userId,

                CreatedAt = DateTime.UtcNow,

                Status = OrderStatus.Processing,

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
                Console.WriteLine(ex.Message);
            }//111111111111111111111111111111111111111111111111111111

            foreach (CartItem item in cart.CartItems)
            {
                foreach (var i in cart.CartItems)
                {
                    Console.WriteLine(
                        $"ITEM: {item.Product?.Name}");

                    Console.WriteLine(
                        $"PRICE: {item.Product?.Price}");
                }

                Console.WriteLine("BEFORE ORDER");

                OrderItem orderItem = new OrderItem
                    {
                        OrderId = order.Id,

                        ProductId = item.ProductId,

                        Quantity = item.Quantity,

                        Price = item.Product.Price
                    };
                Console.WriteLine("AFTER ORDER");

                item.Product.QuantityInStock -=
                    item.Quantity;

                _context.OrderItems.Add(orderItem);
            }

            _context.CartItems.RemoveRange(cart.CartItems);

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }//111111111111111111111111111111111111111111111111111111

            TempData["Success"] = "Order created successfully!";

            return RedirectToAction("MyOrders");
        }
    }
}