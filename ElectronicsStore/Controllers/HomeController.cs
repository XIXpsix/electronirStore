using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ElectronicsStore.Controllers
{
    // C# 12: Основной конструктор
    public class HomeController(IProductService productService) : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Privacy() => View();

        public IActionResult About() => View();

        public IActionResult Contacts() => View();

        [HttpGet]
        public async Task<IActionResult> Catalog(string category, string searchString)
        {
            var response = await productService.GetProducts();

            // C# 12: Выражение коллекции []
            IEnumerable<Product> products = response.Data ?? [];

            if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
            {
                // 1. Фильтрация по Категории
                if (!string.IsNullOrEmpty(category))
                {
                    // Исправлено: StringComparison.OrdinalIgnoreCase для скорости и игнорирования регистра
                    // Исправлено: Проверки p.Category?.Name != null
                    if (category == "tvs")
                        products = products.Where(p => p.Category?.Name != null &&
                            (p.Category.Name.Contains("Телевизор", StringComparison.OrdinalIgnoreCase) ||
                             p.Category.Name.Contains("Монитор", StringComparison.OrdinalIgnoreCase)));
                    else if (category == "pc")
                        products = products.Where(p => p.Category?.Name != null &&
                            (p.Category.Name.Contains("ПК", StringComparison.OrdinalIgnoreCase) ||
                             p.Category.Name.Contains("Ноутбук", StringComparison.OrdinalIgnoreCase)));
                    else
                        products = products.Where(p => p.Category?.Name == category);
                }

                // 2. Поиск
                if (!string.IsNullOrEmpty(searchString))
                {
                    products = products.Where(p => p.Name != null &&
                        p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase));
                }

                return View(products.ToList());
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