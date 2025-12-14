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

            // --- 1. Создание категорий (ОБНОВЛЕННЫЙ СПИСОК) ---
            var categoriesToCreate = new List<Category>
            {
                new Category
                {
                    Name = "Смартфоны", // Оставляем только это
                    Slug = "smartphones",
                    ImagePath = "/img/category_phones.jpg",
                    Description = "Мобильные устройства"
                },
                new Category
                {
                    Name = "Компьютеры", // ОБЪЕДИНЕННАЯ КАТЕГОРИЯ
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
                if (!existingCategories.Any(x => x.Slug == cat.Slug))
                {
                    await categoryRepository.Add(cat);
                }
            }

            var dbCategories = categoryRepository.GetAll().ToList();

            // --- 2. Создание товаров ---
            var currentProducts = await productService.GetProducts();

            if (currentProducts.Data == null || currentProducts.Data.Count() < 15)
            {
                // -- СМАРТФОНЫ --
                var phoneCatId = dbCategories.FirstOrDefault(x => x.Slug == "smartphones")?.Id ?? 0;
                if (phoneCatId != 0)
                {
                    await productService.CreateProduct(new ProductViewModel { Name = "iPhone 15 Pro", Description = "Флагман Apple", Price = 120000, CategoryId = phoneCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Samsung Galaxy S24", Description = "Galaxy AI", Price = 90000, CategoryId = phoneCatId });
                }

                // -- КОМПЬЮТЕРЫ (В одну категорию, но разные названия для фильтра) --
                var compCatId = dbCategories.FirstOrDefault(x => x.Slug == "computers")?.Id ?? 0;
                if (compCatId != 0)
                {
                    // Ноутбуки
                    await productService.CreateProduct(new ProductViewModel { Name = "Ноутбук MacBook Air 15", Description = "Apple M3", Price = 160000, CategoryId = compCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Ноутбук MSI Katana", Description = "Игровой", Price = 125000, CategoryId = compCatId });

                    // ПК
                    await productService.CreateProduct(new ProductViewModel { Name = "ПК ASUS ROG Station", Description = "RTX 4080", Price = 250000, CategoryId = compCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "ПК Office Basic", Description = "Офисный", Price = 40000, CategoryId = compCatId });
                }
            }
        }
    }
}