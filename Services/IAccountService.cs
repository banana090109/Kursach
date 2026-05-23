using BuildStore.Models;
using BuildStore.Models.ViewModels;

namespace BuildStore.Services
{
    public interface IAccountService
    {
        Task<bool>
            EmailExistsAsync(
                string email);

        Task<User>
            CreateUserAsync(
                RegisterViewModel model);

        Task<User?>
            GetByEmailAsync(
                string email);

        Task<User?>
            GetByIdAsync(
                int id);

        bool VerifyPassword(
            string password,
            string passwordHash);

        Task<List<Order>>
            GetUserOrdersAsync(
                int userId);
    }
}