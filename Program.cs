using BuildStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using BuildStore.Services;

namespace BuildStore
{
    public class Program
    {
        public static void Main(string[] args)
        {


            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services
            .AddAuthentication(
                CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options => { options.LoginPath = "/Account/Login"; });
                    
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICartService,CartService>();
            builder.Services.AddScoped<IOrderService,OrderService>();
            builder.Services.AddScoped<IAccountService,AccountService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Product}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
