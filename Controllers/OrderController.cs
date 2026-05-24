using BuildStore.Data;
using BuildStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using BuildStore.Services;

namespace BuildStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult>
            MyOrders()
        {
            string? userIdString =User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdString);
            List<Order> orders =  await _orderService.GetUserOrdersAsync(userId);
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult>
        Checkout()
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdString);
            await _orderService.CheckoutAsync(userId);
            //TempData["Success"] = "Order created successfully!";
            return RedirectToAction("MyOrders");
        }
    }
}