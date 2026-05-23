using BuildStore.Data;
using BuildStore.Models;
using BuildStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BuildStore.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;

        public AccountService(
            AppDbContext context)
        {
            _context = context;
        }

        //public async Task<User?>
        //    LoginAsync(
        //        string email,
        //        string password)
        //{
        //    return await _context.Users

        //        .FirstOrDefaultAsync(u =>
        //            u.Email == email &&
        //            u.PasswordHash == password);
        //}
        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify( password, passwordHash);
        }

        public async Task<bool>
            RegisterAsync(User user)
        {
            bool exists = await _context.Users.AnyAsync(u => u.Email == user.Email);

            if (exists)
            {
                return false;
            }

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<User?>
            GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task
            UpdateProfileAsync(
                User user)
        {
            _context.Users.Update(user);

            await _context.SaveChangesAsync();
        }

        public async Task<bool>
    EmailExistsAsync(
        string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<User>
            CreateUserAsync(
                RegisterViewModel model)
        {
            User user = new User
            {
                Email = model.Email,

                PhoneNumber =
                    model.PhoneNumber,

                PasswordHash =
                    BCrypt.Net.BCrypt
                        .HashPassword(
                            model.Password),

                CreatedAt =
                    DateTime.UtcNow
            };

            _context.Users.Add(user);

            await _context
                .SaveChangesAsync();

            return user;
        }

        public async Task<User?>
            GetByEmailAsync(
                string email)
        {
            return await _context.Users

                .FirstOrDefaultAsync(
                    u => u.Email == email);
        }

        public async Task<User?>
            GetByIdAsync(
                int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<Order>>
            GetUserOrdersAsync(
                int userId)
        {
            return await _context.Orders.Where(o =>o.UserId == userId).ToListAsync();
        }
    }
}