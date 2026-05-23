using BuildStore.Data;
using BuildStore.Models;
using BuildStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BuildStore.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product>?
        GetByIdAsync(int id)
        {
            return 
                await _context.Products.Include(p => p.Categories).Include(p => p.Reviews).ThenInclude(r => r.User).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ProductCatalogViewModel>
            GetCatalogAsync(
                string search,
                string category)
        {
            var productsQuery =
                _context.Products

                .Include(p => p.Categories)

                .Include(p => p.Reviews)

                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                productsQuery =
                    productsQuery.Where(p =>

                        p.Name.ToLower()
                        .Contains(search.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                productsQuery =
                    productsQuery.Where(p =>

                        p.Categories.Any(c =>
                            c.Name == category));
            }

            ProductCatalogViewModel
                viewModel =
                    new ProductCatalogViewModel
                    {
                        Products =
                            await productsQuery
                                .ToListAsync(),

                        Categories =
                            await _context.Categories

                                .OrderBy(c => c.Name)

                                .ToListAsync(),

                        Search = search,

                        SelectedCategory = category
                    };

            return viewModel;
        }
    }
}