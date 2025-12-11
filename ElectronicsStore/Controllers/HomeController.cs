using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks; // Не забудьте добавить это для async/await

namespace ElectronicsStore.Controllers
{
    public class HomeController(IProductService productService, ICategoryService categoryService) : Controller
    {
        // Изменили метод на async Task и загружаем товары
        public async Task<IActionResult> Index()
        {
            // Получаем список всех товаров
            var response = await productService.GetProducts();

            // Если успех - передаем товары, иначе пустой список
            var products = response.StatusCode == Domain.Enum.StatusCode.OK
                ? response.Data
                : new List<Product>();

            return View(products);
        }

        public IActionResult Privacy() => View();
        public IActionResult About() => View();
        public IActionResult Contacts() => View();

        [HttpGet]
        public async Task<IActionResult> Catalog(string? category, string? searchString)
        {
            var productsResponse = await productService.GetProducts();
            IEnumerable<Product> products = productsResponse.Data ?? [];

            var categoriesResponse = await categoryService.GetCategories();
            IEnumerable<Category> categories = categoriesResponse.Data ?? [];

            if (productsResponse.StatusCode == Domain.Enum.StatusCode.OK)
            {
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

            return Content($"ОШИБКА: {productsResponse.Description}");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}