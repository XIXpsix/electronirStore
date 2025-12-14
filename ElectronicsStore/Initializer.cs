using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace ElectronicsStore
{
    public static class Initializer
    {
        public static async Task InitializeData(IServiceProvider serviceProvider)
        {
            var productService = serviceProvider.GetRequiredService<IProductService>();
            var categoryRepository = serviceProvider.GetRequiredService<IBaseStorage<Category>>();

            // --- КАТЕГОРИИ (Без "Телефонов", только Смартфоны и Компьютеры) ---
            var categoriesToCreate = new List<Category>
            {
                new Category
                {
                    Name = "Смартфоны",
                    Slug = "smartphones",
                    ImagePath = "/img/category_phones.jpg",
                    Description = "Мобильные устройства"
                },
                new Category
                {
                    Name = "Компьютеры", // Единая папка
                    Slug = "computers",
                    ImagePath = "/img/category_laptops.jpg",
                    Description = "Ноутбуки и ПК"
                },
                new Category
                {
                    Name = "Мониторы",
                    Slug = "monitors",
                    ImagePath = "/img/category_monitors.jpg",
                    Description = "Мониторы и дисплеи"
                },
                new Category
                {
                    Name = "Периферия",
                    Slug = "accessories",
                    ImagePath = "/img/category_accessories.jpg",
                    Description = "Клавиатуры, мыши и прочее"
                },
                new Category
                {
                    Name = "Планшеты",
                    Slug = "tablets",
                    ImagePath = "/img/category_tablets.jpg",
                    Description = "Планшетные компьютеры"
                }
            };

            var existingCategories = categoryRepository.GetAll().ToList();

            foreach (var cat in categoriesToCreate)
            {
                // Добавляем, если нет (проверка по Slug)
                if (!existingCategories.Any(x => x.Slug == cat.Slug))
                {
                    await categoryRepository.Add(cat);
                }
            }

            var dbCategories = categoryRepository.GetAll().ToList();

            // --- ТОВАРЫ ---
            var currentProducts = await productService.GetProducts();

            if (currentProducts.Data == null || !currentProducts.Data.Any())
            {
                // Смартфоны
                var phoneCatId = dbCategories.FirstOrDefault(x => x.Slug == "smartphones")?.Id ?? 0;
                if (phoneCatId != 0)
                {
                    await productService.CreateProduct(new ProductViewModel { Name = "iPhone 15 Pro", Description = "Флагман Apple", Price = 120000, CategoryId = phoneCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Samsung Galaxy S24", Description = "AI Phone", Price = 90000, CategoryId = phoneCatId });
                }

                // Компьютеры (Все вместе)
                var compCatId = dbCategories.FirstOrDefault(x => x.Slug == "computers")?.Id ?? 0;
                if (compCatId != 0)
                {
                    await productService.CreateProduct(new ProductViewModel { Name = "MacBook Air 15", Description = "M3 Chip", Price = 160000, CategoryId = compCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "MSI Katana", Description = "Игровой ноутбук", Price = 125000, CategoryId = compCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "ASUS ROG Station", Description = "Мощный ПК", Price = 250000, CategoryId = compCatId });
                }
            }
        }
    }
}