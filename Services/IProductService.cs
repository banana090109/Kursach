using BuildStore.Models;
using BuildStore.Models.ViewModels;

namespace BuildStore.Services
{
    public interface IProductService
    {
        Task<ProductCatalogViewModel>
            GetCatalogAsync(
                string search,
                string category);

        Task<Product> GetByIdAsync(int id);
    }
}