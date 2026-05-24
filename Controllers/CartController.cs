using BuildStore.Data;
using BuildStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BuildStore.Services;

namespace BuildStore.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdString);
            var cart = await _cartService.GetCartAsync(userId);
            return View(cart);
        }

        public async Task<IActionResult> Increase(int itemId)
        {
            await _cartService.IncreaseAsync(itemId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Decrease(int itemId)
        {
            await _cartService.DecreaseAsync(itemId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int itemId)
        {
            await _cartService.RemoveAsync(itemId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddAjax(int productId)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userId = int.Parse(userIdString);
            var (success, message) = await _cartService.AddAjaxAsync(userId, productId);
            return Json(new { success, message });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
            {
                return Json(0);
            }
            int userId = int.Parse(userIdString);
            int count = await _cartService.GetCartCountAsync(userId);
            return Json(count);
        }
    }
}


//using BuildStore.Data;
//using BuildStore.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authorization;
//using System.Security.Claims;
//using BuildStore.Services;

//namespace BuildStore.Controllers
//{
//    [Authorize]
//    public class CartController : Controller
//    {
//        private readonly ICartService _cartService;

//        public CartController(
//            ICartService cartService)
//        {
//            _cartService = cartService;
//        }

//        public async Task<IActionResult> Index()
//        {
//            string? userIdString =User.FindFirstValue(ClaimTypes.NameIdentifier);
//            int userId = int.Parse(userIdString);
//            var cart = await _cartService.GetCartAsync(userId);
//            return View(cart);
//        }

//        public async Task<IActionResult> Add(int productId)
//        {
//            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            int userId =int.Parse(userIdString);
//            await _cartService.AddToCartAsync(userId,productId);
//            //TempData["Success"] = "Product added to cart!";
//            return Redirect(Request.Headers["Referer"].ToString());
//        }

//        public async Task<IActionResult> Increase(int itemId)
//        {
//            await _cartService.IncreaseAsync(itemId);
//            return RedirectToAction("Index");
//        }
//        public async Task<IActionResult> Decrease(int itemId)
//        {
//            await _cartService.DecreaseAsync(itemId);
//            return RedirectToAction("Index");
//        }
//        public async Task<IActionResult> Remove(int itemId)
//        {
//            await _cartService.RemoveAsync(itemId);
//            return RedirectToAction("Index");
//        }

//        [HttpPost]
//        [HttpPost]
//        public async Task<IActionResult> AddAjax(int productId)
//        {
//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            var (success, message) = await _cartService.AddAjaxAsync(userId, productId);

//            return Json(new { success, message });
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetCartCount()
//        {
//            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            if (userIdString == null)
//            {
//                return Json(0);
//            }
//            int userId = int.Parse(userIdString);
//            int count = await _cartService.GetCartCountAsync(userId);
//            return Json(count);
//        }
//    }
//}