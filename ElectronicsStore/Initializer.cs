using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.BLL.Realizations;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.DAL.Repositories;
using ElectronicsStore.Domain.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicsStore
{
    public static class Initializer
    {
        public static void InitializeRepositories(IServiceCollection services)
        {
            services.AddScoped<IBaseStorage<User>, UserStorage>();
            services.AddScoped<IBaseStorage<Product>, ProductStorage>();
            services.AddScoped<IBaseStorage<Category>, CategoryStorage>();
            services.AddScoped<IBaseStorage<Review>, ReviewStorage>();
            services.AddScoped<IBaseStorage<ProductImage>, ProductImageStorage>();

            // Добавляем хранилища для корзины
            services.AddScoped<IBaseStorage<Cart>, CartStorage>();
            services.AddScoped<IBaseStorage<CartItem>, CartItemStorage>();
        }

        public static void InitializeServices(IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IEmailService, EmailService>();

            // Добавляем сервис корзины
            services.AddScoped<ICartService, CartService>();
        }
    }
}