using BuildStore.Data;
using BuildStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BuildStore.Models.ViewModels;
using System.Threading.Tasks;
using System.Security.Claims;
using BuildStore.Services;

namespace BuildStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService
            _productService;

        public ProductController(
            IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(
            string search,
            string category)
        {
            ProductCatalogViewModel viewModel =
                await _productService
                    .GetCatalogAsync(
                        search,
                        category);

            return View(viewModel);
        }
        public async Task<IActionResult>
    Details(int id)
        {
            Product product =
                await _productService
                    .GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

    }
}