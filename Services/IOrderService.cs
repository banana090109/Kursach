using BuildStore.Models;

namespace BuildStore.Services
{
    public interface IOrderService
    {
        Task<List<Order>>
            GetUserOrdersAsync(
                int userId);

        Task CheckoutAsync(
            int userId);
    }
}