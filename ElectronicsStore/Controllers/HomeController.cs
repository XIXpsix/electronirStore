using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ElectronicsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService; // Оставил, если вдруг понадобится, хотя сейчас не используется

        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public IActionResult Index() => View();

        public IActionResult Privacy() => View();

        public IActionResult About() => View();

        public IActionResult Contacts() => View();

        [HttpGet]
        public async Task<IActionResult> Catalog(string category, string searchString)
        {
            var response = await _productService.GetProducts();

            // Если Data придет null, используем пустой список, чтобы не упало ниже
            IEnumerable<Product> products = response.Data ?? new List<Product>();

            if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
            {
                // 1. Фильтрация по Категории
                if (!string.IsNullOrEmpty(category))
                {
                    if (category == "tvs")
                    {
                        // ИСПРАВЛЕНИЕ: Добавлена проверка p.Category.Name != null
                        products = products.Where(p => p.Category != null && p.Category.Name != null &&
                            (p.Category.Name.Contains("Телевизор") || p.Category.Name.Contains("Монитор")));
                    }
                    else if (category == "pc")
                    {
                        // ИСПРАВЛЕНИЕ: Добавлена проверка p.Category.Name != null
                        products = products.Where(p => p.Category != null && p.Category.Name != null &&
                            (p.Category.Name.Contains("ПК") || p.Category.Name.Contains("Ноутбук")));
                    }
                    else
                    {
                        products = products.Where(p => p.Category != null && p.Category.Name == category);
                    }
                }

                // 2. Поиск
                if (!string.IsNullOrEmpty(searchString))
                {
                    // ИСПРАВЛЕНИЕ: Проверка, что имя продукта не null перед поиском
                    products = products.Where(p => p.Name != null && p.Name.ToLower().Contains(searchString.ToLower()));
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