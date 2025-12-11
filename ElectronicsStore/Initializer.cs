using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.BLL.Realizations;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.DAL.Repositories;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.ViewModels;
using ElectronicsStore.Domain.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Нужно для работы ToListAsync()

namespace ElectronicsStore
{
    public static class Initializer
    {
        // --- ТВОЙ СТАРЫЙ КОД (Регистрация сервисов) ---
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

        // --- НОВЫЙ КОД (Заполнение базы данными) ---
        public static async Task InitializeData(IBaseStorage<User> userStorage,
                                          IBaseStorage<Product> productStorage,
                                          IBaseStorage<Category> categoryStorage)
        {
            // 1. Создаем КАТЕГОРИИ с правильными Slug
            var categories = await categoryStorage.GetAll().ToListAsync();
            if (!categories.Any())
            {
                var newCategories = new List<Category>
                {
                    new Category { Name = "Телефоны", Slug = "smartphones", ImagePath = "/img/iphone15.jpg" },
                    new Category { Name = "ТВ и Экраны", Slug = "tvs", ImagePath = "/img/monitor.jpg" },
                    new Category { Name = "Компьютеры", Slug = "pc", ImagePath = "/img/laptop.jpg" },
                    new Category { Name = "Аксессуары", Slug = "accessories", ImagePath = "/img/mouse.jpg" }
                };

                foreach (var cat in newCategories)
                {
                    await categoryStorage.Add(cat);
                }

                // Перечитываем базу, чтобы получить ID новых категорий
                categories = await categoryStorage.GetAll().ToListAsync();
            }

            // Находим ID для привязки
            var pcId = categories.FirstOrDefault(c => c.Slug == "pc")?.Id ?? 0;
            var phoneId = categories.FirstOrDefault(c => c.Slug == "smartphones")?.Id ?? 0;
            var accId = categories.FirstOrDefault(c => c.Slug == "accessories")?.Id ?? 0;
            var tvId = categories.FirstOrDefault(c => c.Slug == "tvs")?.Id ?? 0;

            // 2. Создаем ТОВАРЫ, если их нет
            if (!await productStorage.GetAll().AnyAsync())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Iphone 15 Pro",
                        Description = "Титановый корпус, чип A17 Pro.",
                        Price = 120000,
                        CategoryId = phoneId,
                        CreatedAt = DateTime.Now,
                        ImagePath = "/img/iphone15.jpg"
                    },
                    new Product
                    {
                        Name = "Samsung Galaxy S24 Ultra",
                        Description = "Искусственный интеллект Galaxy AI.",
                        Price = 115000,
                        CategoryId = phoneId,
                        CreatedAt = DateTime.Now,
                        ImagePath = "/img/iphone15.jpg" // Замени на свое фото
                    },
                    new Product
                    {
                        Name = "Asus ROG Strix G16",
                        Description = "Мощный игровой ноутбук с RTX 4060.",
                        Price = 185000,
                        CategoryId = pcId,
                        CreatedAt = DateTime.Now,
                        ImagePath = "/img/laptop.jpg" // Убедись, что картинка есть
                    },
                    new Product
                    {
                        Name = "Logitech G Pro X Superlight",
                        Description = "Самая легкая киберспортивная мышь.",
                        Price = 14000,
                        CategoryId = accId,
                        CreatedAt = DateTime.Now,
                        ImagePath = "/img/mouse.jpg"
                    },
                    new Product
                    {
                        Name = "LG OLED C3 55\"",
                        Description = "Лучший OLED телевизор для игр.",
                        Price = 150000,
                        CategoryId = tvId,
                        CreatedAt = DateTime.Now,
                        ImagePath = "/img/monitor.jpg"
                    }
                };

                foreach (var product in products)
                {
                    if (product.CategoryId != 0) // Добавляем только если категория нашлась
                    {
                        await productStorage.Add(product);
                    }
                }
            }
        }
    }
}