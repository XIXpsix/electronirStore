using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.BLL.Realizations;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.DAL.Realizations;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicsStore
{
    public static class ServiceExtensions
    {
        // ? ИСПРАВЛЕНИЕ: Метод для инициализации repositories
        public static void InitializeRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseStorage<>), typeof(BaseStorage<>));
        }

        // ? ИСПРАВЛЕНИЕ: Метод для инициализации сервисов
        public static void InitializeServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IEmailService, EmailService>();
        }

        // ? ИСПРАВЛЕНИЕ: Метод для инициализации валидаторов
        public static void InitializeValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<RegisterViewModel>, RegisterValidator>();
            services.AddScoped<IValidator<ProductViewModel>, ProductValidator>();
            services.AddValidatorsFromAssemblyContaining<RegisterValidator>();
        }
    }
}