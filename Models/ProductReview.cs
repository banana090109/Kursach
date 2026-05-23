namespace BuildStore.Models
{
    public class ProductReview
    {
        public int Id { get; set; }

        public int Rating { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}