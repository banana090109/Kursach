using BuildStore.Data;
using BuildStore.Models;
using Microsoft.AspNetCore.Mvc;
using BuildStore.Models.ViewModels;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BuildStore.Services;

namespace BuildStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction(
                    "Login");
            }

            string? userIdString =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            int userId = int.Parse(userIdString);

            User? user = await _accountService.GetByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction(
                    "Login");
            }

            List<Order> orders = await _accountService.GetUserOrdersAsync(userId);

            ViewBag.TotalOrders = orders.Count;

            ViewBag.TotalSpent = orders.Sum(o =>
                o.TotalPrice);

            return View(user);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool emailExists = await _accountService.EmailExistsAsync(model.Email);

            if (emailExists)
            {
                ModelState.AddModelError(
                    "",
                    "User with this email already exists.");

                return View(model);
            }

            await _accountService
                .CreateUserAsync(model);

            return RedirectToAction(
                "Login",
                "Account");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction(
                "Index",
                "Product");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User? user =
    await _accountService
        .GetByEmailAsync(
            model.Email);

            if (user == null)
            {
                ModelState.AddModelError(
                    "",
                    "Invalid email or password.");

                return View(model);
            }

            bool validPassword =
                BCrypt.Net.BCrypt.Verify(
                    model.Password,
                    user.PasswordHash);

            if (!validPassword)
            {
                ModelState.AddModelError(
                    "",
                    "Invalid email or password.");

                return View(model);
            }

            List<Claim> claims = new List<Claim>
            {
            new Claim(ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(ClaimTypes.Email,
                user.Email)
            };

                ClaimsIdentity identity =
                    new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults
                            .AuthenticationScheme);

                ClaimsPrincipal principal =
                    new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults
                        .AuthenticationScheme,

                    principal);

                return RedirectToAction(
                    "Index",
                    "Product");
            }
    }
}