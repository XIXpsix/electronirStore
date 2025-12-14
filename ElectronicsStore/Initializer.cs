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
    public class Initializer
    {
        public static async Task InitializeRepositories(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            var categoryRepository = scope.ServiceProvider.GetRequiredService<IBaseStorage<Category>>();

            // --- 1. Создание категорий ---
            var categoriesToCreate = new List<Category>
            {
                new Category 
                { 
                    Name = "Телефоны", 
                    Slug = "smartphones",
                    ImagePath = "/img/category_phones.jpg",
                    Description = "Мобильные телефоны"
                },
                new Category 
                { 
                    Name = "Компьютеры", 
                    Slug = "laptops",
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
                    Description = "Периферийные устройства"
                },
                new Category 
                { 
                    Name = "Планшеты", 
                    Slug = "tablets",
                    ImagePath = "/img/category_tablets.jpg",
                    Description = "Планшетные компьютеры"
                    }
            };

            // Получаем уже существующие категории из БД
            var existingCategories = categoryRepository.GetAll().ToList();

            foreach (var cat in categoriesToCreate)
            {
                // Если категории с таким именем нет в БД — добавляем
                if (!existingCategories.Any(x => x.Name == cat.Name))
                {
                    await categoryRepository.Add(cat);
                }
            }

            // Обновляем список категорий из БД (чтобы получить их ID)
            var dbCategories = categoryRepository.GetAll().ToList();

            // --- 2. Создание товаров (если их мало) ---
            var currentProducts = await productService.GetProducts();
            
            // ✅ ИСПРАВЛЕНИЕ: Проверка на null перед Count()
            if (currentProducts.Data == null || currentProducts.Data.Count() < 15)
            {
                // -- ТЕЛЕФОНЫ --
                var phoneCatId = dbCategories.FirstOrDefault(x => x.Name == "Телефоны")?.Id ?? 0;
                if (phoneCatId != 0)
                {
                    await productService.CreateProduct(new ProductViewModel { Name = "iPhone 15 Pro", Description = "Флагман Apple, титановый корпус", Price = 120000, CategoryId = phoneCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Samsung Galaxy S24", Description = "Искусственный интеллект Galaxy AI", Price = 90000, CategoryId = phoneCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Xiaomi 14 Ultra", Description = "Профессиональная камера Leica", Price = 100000, CategoryId = phoneCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Google Pixel 8", Description = "Чистый Android и лучшие фото", Price = 75000, CategoryId = phoneCatId });
                }

                // -- КОМПЬЮТЕРЫ --
                var pcCatId = dbCategories.FirstOrDefault(x => x.Name == "Компьютеры")?.Id ?? 0;
                if (pcCatId != 0)
                {
                    await productService.CreateProduct(new ProductViewModel { Name = "MacBook Air 15", Description = "Чип M3, легкий и мощный", Price = 160000, CategoryId = pcCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "ASUS TUF Gaming", Description = "RTX 4060, для игр и работы", Price = 110000, CategoryId = pcCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "MSI Katana", Description = "Мощная игровая станция", Price = 125000, CategoryId = pcCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Lenovo Legion Pro", Description = "Киберспортивный ноутбук", Price = 180000, CategoryId = pcCatId });
                }

                // -- МОНИТОРЫ --
                var monCatId = dbCategories.FirstOrDefault(x => x.Name == "Мониторы")?.Id ?? 0;
                if (monCatId != 0)
                {
                    await productService.CreateProduct(new ProductViewModel { Name = "Samsung Odyssey G5", Description = "Изогнутый 34 дюйма, 165Гц", Price = 45000, CategoryId = monCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "LG UltraGear 27", Description = "Nano IPS, 1мс отклик", Price = 38000, CategoryId = monCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Xiaomi Mi Monitor", Description = "34 дюйма, бюджетный ultrawide", Price = 30000, CategoryId = monCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Dell Alienware", Description = "OLED 360Гц, мечта геймера", Price = 110000, CategoryId = monCatId });
                }

                // -- ПЕРИФЕРИЯ --
                var perCatId = dbCategories.FirstOrDefault(x => x.Name == "Периферия")?.Id ?? 0;
                if (perCatId != 0)
                {
                    await productService.CreateProduct(new ProductViewModel { Name = "Logitech G Pro X", Description = "Беспроводная мышь, суперлегкая", Price = 14000, CategoryId = perCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Keychron K2", Description = "Механическая клавиатура", Price = 12000, CategoryId = perCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "HyperX Cloud Alpha", Description = "Легендарная игровая гарнитура", Price = 9000, CategoryId = perCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Razer BlackWidow", Description = "Клавиатура с подсветкой Chroma", Price = 15000, CategoryId = perCatId });
                }

                // -- ПЛАНШЕТЫ --
                var tabCatId = dbCategories.FirstOrDefault(x => x.Name == "Планшеты")?.Id ?? 0;
                if (tabCatId != 0)
                {
                    await productService.CreateProduct(new ProductViewModel { Name = "iPad Air M1", Description = "Мощность ПК в тонком корпусе", Price = 65000, CategoryId = tabCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Samsung Tab S9", Description = "Экран AMOLED 120Гц", Price = 80000, CategoryId = tabCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Xiaomi Pad 6", Description = "Топ за свои деньги", Price = 35000, CategoryId = tabCatId });
                    await productService.CreateProduct(new ProductViewModel { Name = "Huawei MatePad", Description = "Отличный экран и звук", Price = 30000, CategoryId = tabCatId });
                }
            }
        }

        internal static async Task InitializeData(IBaseStorage<User> userStorage, IBaseStorage<Product> productStorage, IBaseStorage<Category> categoryStorage)
        {
            throw new NotImplementedException();
        }
    }
}