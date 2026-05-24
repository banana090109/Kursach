using BuildStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildStore.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ElectricalTool> ElectricalTools { get; set; }

        public DbSet<HandTool> HandTools { get; set; }

        public DbSet<ConstructionMaterial> ConstructionMaterials { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<ProductReview> ProductReviews { get; set; }
    }
}