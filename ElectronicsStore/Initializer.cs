using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.BLL.Realizations;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.DAL.Repositories;
using ElectronicsStore.Domain.Entity;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicsStore
{
    public static class Initializer
    {
        // Добавлено слово 'this' перед IServiceCollection
        public static void InitializeRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBaseStorage<User>, UserStorage>();
            services.AddScoped<IBaseStorage<Product>, ProductStorage>();
            services.AddScoped<IBaseStorage<Category>, CategoryStorage>();
            services.AddScoped<IBaseStorage<Review>, ReviewStorage>();
            services.AddScoped<IBaseStorage<ProductImage>, ProductImageStorage>();
            services.AddScoped<IBaseStorage<Cart>, CartStorage>();
            services.AddScoped<IBaseStorage<CartItem>, CartItemStorage>();
        }

        // Добавлено слово 'this' перед IServiceCollection
        public static void InitializeServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICartService, CartService>();
        }
    }
}