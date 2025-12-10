using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.BLL.Realizations;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.DAL.Repositories;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.ViewModels;
using ElectronicsStore.Domain.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicsStore
{
    public static class Initializer
    {
        public static void InitializeRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBaseStorage<User>, UserStorage>();
            services.AddScoped<IBaseStorage<Cart>, CartStorage>();
            services.AddScoped<IBaseStorage<CartItem>, CartItemStorage>();
            services.AddScoped<IBaseStorage<Product>, ProductStorage>();
            services.AddScoped<IBaseStorage<Category>, CategoryStorage>();
            services.AddScoped<IBaseStorage<ProductImage>, ProductImageStorage>();
            services.AddScoped<IBaseStorage<Review>, ReviewStorage>();
        }

        public static void InitializeServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
        }

        public static void InitializeValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<RegisterViewModel>, RegisterValidator>();
            services.AddScoped<IValidator<LoginViewModel>, LoginValidator>();
            services.AddScoped<IValidator<ProductViewModel>, ProductValidator>();
        }
    }
}