using BuildStore.Models;

namespace BuildStore.Models.ViewModels
{
    public class ProductCatalogViewModel
    {
        public List<Product> Products { get; set; }

        public List<Category> Categories { get; set; }

        public string Search { get; set; }

        public string SelectedCategory { get; set; }
    }
}