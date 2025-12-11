using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Collections.Generic; // Добавлено для IEnumerable
using System.Linq; // Добавлено для Where и ToList
using System; // Добавлено для StringComparison

namespace ElectronicsStore.Controllers
{
    // ИСПРАВЛЕНО: Использование основного конструктора для IProductService и ICategoryService
    public class HomeController(IProductService productService, ICategoryService categoryService) : Controller
    {
        // Инжектированные сервисы (productService, categoryService) используются напрямую

        public IActionResult Index() => View();
        public IActionResult Privacy() => View();
        public IActionResult About() => View();
        public IActionResult Contacts() => View();

        [HttpGet]
        // ИСПРАВЛЕНО NRT: category и searchString могут быть null (string?)
        public async Task<IActionResult> Catalog(string? category, string? searchString)
        {
            // 1. Получаем продукты
            var productsResponse = await productService.GetProducts();
            // ИСПРАВЛЕНО NRT: Используем ?? [] для пустой коллекции (requires C# 12, or switch to new List<Product>())
            IEnumerable<Product> products = productsResponse.Data ?? [];

            // 2. Получаем категории (для CatalogViewModel)
            var categoriesResponse = await categoryService.GetCategories();
            IEnumerable<Category> categories = categoriesResponse.Data ?? [];

            // Если возникла ошибка при получении продуктов (хотя категории могли быть получены)
            if (productsResponse.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
            {
                // Применяем фильтр по категории
                if (!string.IsNullOrEmpty(category))
                {
                    // ИСПРАВЛЕНИЕ: Проверяем p.Category?.Name != null
                    if (category == "tvs")
                        products = products.Where(p => p.Category?.Name != null &&
                            (p.Category.Name.Contains("Телевизор", StringComparison.OrdinalIgnoreCase) ||
                             p.Category.Name.Contains("Монитор", StringComparison.OrdinalIgnoreCase)));
                    else if (category == "pc")
                        products = products.Where(p => p.Category?.Name != null &&
                            (p.Category.Name.Contains("ПК", StringComparison.OrdinalIgnoreCase) ||
                             p.Category.Name.Contains("Ноутбук", StringComparison.OrdinalIgnoreCase)));
                    else
                        // ИСПРАВЛЕНИЕ: Фильтруем по Slug категории
                        products = products.Where(p => p.Category?.Slug == category);
                }

                // Применяем фильтр по поисковой строке
                if (!string.IsNullOrEmpty(searchString))
                {
                    // ИСПРАВЛЕНО NRT: Проверяем p.Name != null
                    products = products.Where(p => p.Name != null &&
                        p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));
                }

                // ИСПРАВЛЕНО: Создаем и возвращаем CatalogViewModel. 
                // Это решит ошибки "CatalogViewModel" не содержит определения "GroupBy" и "Any" в представлении.
                var viewModel = new CatalogViewModel
                {
                    Products = products.ToList(),
                    Categories = categories.ToList(),
                    CurrentSearchName = searchString,
                    CurrentCategoryId = categories.FirstOrDefault(c => c.Slug == category)?.Id ?? 0 // Опционально: устанавливаем ID текущей категории
                };

                return View(viewModel);
            }
            return RedirectToAction("Error");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}