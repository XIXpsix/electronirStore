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
        // Метод заполнения данными
        public static async Task InitializeData(IServiceProvider serviceProvider)
        {
            var productService = serviceProvider.GetRequiredService<IProductService>();
            var categoryRepository = serviceProvider.GetRequiredService<IBaseStorage<Category>>();

            // --- 1. Создание категорий ---
            // Мы делим компьютеры на Ноутбуки и ПК, и переименовываем Телефоны в Смартфоны
            var categoriesToCreate = new List<Category>
            {
                new Category
                {
                    Name = "Смартфоны", // Было "Телефоны"
                    Slug = "smartphones",
                    ImagePath = "/img/category_phones.jpg",
                    Description = "Мобильные устройства"
                },
                new Category
                {
                    Name = "Ноутбуки", // Новая категория
                    Slug = "laptops",
                    ImagePath = "/img/category_laptops.jpg",
                    Description = "Компактные компьютеры"
                },
                new Category
                {
                    Name = "Настольные ПК", // Новая категория
                    Slug = "desktops",
                    ImagePath = "/img/category_pc.jpg",
                    Description = "Мощные стационарные компьютеры"
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
                // Проверяем по Slug (уникальному коду), чтобы не дублировать
                if (!existingCategories.Any(x => x.Slug == cat.Slug))
                {
                    await categoryRepository.Add(cat);
                }
            }

            // Обновляем список из БД
            var dbCategories = categoryRepository.GetAll().ToList();

            // --- 2. Создание товаров ---
            var currentProducts = await productService.GetProducts();

            // Если товаров мало, добавляем новые
            if (currentProducts.Data == null || currentProducts.Data.Count() < 15)
            {
                // -- СМАРТФОНЫ --
                var phoneCatId = dbCategories.FirstOrDefault(x => x.Slug == "smartphones")?.Id ?? 0;
                if (phoneCatId != 0)
                {
                    await productService.CreateProduct(new ProductViewModel { Name = "iPhone 15 Pro", Description = "Флагман Apple", Price = 120000, CategoryId = phoneCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Samsung Galaxy S24", Description = "Galaxy AI", Price = 90000, CategoryId = phoneCatId });
                }

                // -- НОУТБУКИ --
                var laptopCatId = dbCategories.FirstOrDefault(x => x.Slug == "laptops")?.Id ?? 0;
                if (laptopCatId != 0)
                {
                    await productService.CreateProduct(new ProductViewModel { Name = "MacBook Air 15", Description = "Чип M3", Price = 160000, CategoryId = laptopCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "MSI Katana", Description = "Игровой ноутбук", Price = 125000, CategoryId = laptopCatId });
                }

                // -- НАСТОЛЬНЫЕ ПК --
                var pcCatId = dbCategories.FirstOrDefault(x => x.Slug == "desktops")?.Id ?? 0;
                if (pcCatId != 0)
                {
                    await productService.CreateProduct(new ProductViewModel { Name = "ASUS ROG Station", Description = "RTX 4080, i9-14900K", Price = 250000, CategoryId = pcCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Office PC Basic", Description = "Для офиса", Price = 40000, CategoryId = pcCatId });
                }

                // ... можно добавить товары для остальных категорий
            }
        }
    }
}