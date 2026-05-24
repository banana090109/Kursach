using BuildStore.Models;

namespace BuildStore.Services
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(int userId);

        //Task AddToCartAsync(int userId,int productId);

        Task IncreaseAsync(int itemId);

        Task DecreaseAsync(int itemId);

        Task RemoveAsync(int itemId);

        Task<(bool Success, string Message)> AddAjaxAsync(int userId, int productId);

        Task<int> GetCartCountAsync(int userId);
    }
}