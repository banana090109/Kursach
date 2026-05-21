namespace BuildStore.Models
{
    public abstract class Product
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public int QuantityInStock { get; set; }

        public string ImagePath { get; set; }

        public List<Category> Categories { get; set; }
    }
}