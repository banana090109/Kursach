using BuildStore.Data;
using BuildStore.Models;
using Microsoft.AspNetCore.Mvc;
using BuildStore.Models.ViewModels;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BuildStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public IActionResult Profile()
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

            User? user = _context.Users
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return RedirectToAction(
                    "Login");
            }

            List<Order> orders = _context.Orders
                .Where(o => o.UserId == userId)
                .ToList();

            ViewBag.TotalOrders = orders.Count;

            ViewBag.TotalSpent = orders.Sum(o =>
                o.TotalPrice);

            return View(user);
        }

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool emailExists = _context.Users
                .Any(u => u.Email == model.Email);

            if (emailExists)
            {
                ModelState.AddModelError(
                    "",
                    "User with this email already exists.");

                return View(model);
            }

            User user = new User
            {
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,

                PasswordHash = BCrypt.Net.BCrypt
                    .HashPassword(model.Password),

                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);

            _context.SaveChanges();

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

            User? user = _context.Users
                .FirstOrDefault(u => u.Email == model.Email);

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