using BuildStore.Data;
using BuildStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildStore.Models.ViewModels;
using System.Threading.Tasks;

namespace BuildStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public async /*типу асинхроний*/ Task<IActionResult> Index(string search, string category) 
        {
            var productsQuery = _context.Products.Include(p => p.Categories).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                productsQuery = productsQuery.Where(p => p.Name.ToLower().Contains(search.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                productsQuery = productsQuery.Where(p => p.Categories.Any(c => c.Name == category));
            }

            var viewModel = new ProductCatalogViewModel
            {
                Products = await productsQuery.ToListAsync(),

                Categories = await _context.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync(),

                Search = search,

                SelectedCategory = category
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ElectricalTool product)
        {
            if (ModelState.IsValid)
            {
                _context.ElectricalTools.Add(product);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(product);
        }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        public async Task<IActionResult> Category(string name)
        {
            var products = await _context.Products
                .Include(p => p.Categories)
                .Where(p => p.Categories.Any(c => c.Name == name))
                .ToListAsync();

            ViewBag.CategoryName = name;

            return View(products);
        }
    }
}