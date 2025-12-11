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

        public IActionResult Index()
        {
            // Передаем пустой список продуктов, чтобы Model в представлении не был null
            return View(new List<Product>());
        }
        public IActionResult Privacy() => View();
        public IActionResult About() => View();
        public IActionResult Contacts() => View();

        [HttpGet]
        public async Task<IActionResult> Catalog(string? category, string? searchString)
        {
            // 1. Получаем продукты
            var productsResponse = await productService.GetProducts();
            IEnumerable<Product> products = productsResponse.Data ?? [];

            // 2. Получаем категории
            var categoriesResponse = await categoryService.GetCategories();
            IEnumerable<Category> categories = categoriesResponse.Data ?? [];

            // Если возникла ошибка при получении продуктов
            if (productsResponse.StatusCode == Domain.Enum.StatusCode.OK)
            {
                // ... (весь ваш код фильтрации, который был внутри if) ...
                if (!string.IsNullOrEmpty(category))
                {
                    if (category == "tvs")
                        products = products.Where(p => p.Category?.Name != null &&
                            (p.Category.Name.Contains("Телевизор", StringComparison.OrdinalIgnoreCase) ||
                             p.Category.Name.Contains("Монитор", StringComparison.OrdinalIgnoreCase)));
                    else if (category == "pc")
                        products = products.Where(p => p.Category?.Name != null &&
                            (p.Category.Name.Contains("ПК", StringComparison.OrdinalIgnoreCase) ||
                             p.Category.Name.Contains("Ноутбук", StringComparison.OrdinalIgnoreCase)));
                    else
                        products = products.Where(p => p.Category?.Slug == category);
                }

                if (!string.IsNullOrEmpty(searchString))
                {
                    products = products.Where(p => p.Name != null &&
                        p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));
                }

                var viewModel = new CatalogViewModel
                {
                    Products = products.ToList(),
                    Categories = categories.ToList(),
                    CurrentSearchName = searchString,
                    CurrentCategoryId = categories.FirstOrDefault(c => c.Slug == category)?.Id ?? 0
                };

                return View(viewModel);
            }

            // --- ИЗМЕНЕНИЕ ЗДЕСЬ ---
            // Вместо RedirectToAction("Error"), выводим текст ошибки, чтобы понять причину
            return Content($"ОШИБКА: {productsResponse.Description}");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}