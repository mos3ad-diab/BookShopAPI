using BookShopAPI.Models;
using BookShopAPI.Repositories;
using BookShopAPI.Utilities;
using BookShopAPI.Utilities.DBSeeder;
using Cinema_website.Utilities.DBSeeder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BookShopAPI
{
    public class ApplicationConfiguration
    {
        public static void RegisterConfig(IServiceCollection services)
        {
            services.AddScoped<IRepository<Book>, Repository<Book>>();
            services.AddScoped<IRepository<Cart>, Repository<Cart>>();
            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<Author>, Repository<Author>>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
            services.AddScoped<IDBInitialization, DBInitialization>();
        }
    }
}
