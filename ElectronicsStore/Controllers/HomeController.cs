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
        private readonly ICategoryService _categoryService;

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

            // Инициализируем пустым списком, если пришел null
            IEnumerable<Product> products = response.Data ?? new List<Product>();

            // ИСПРАВЛЕНИЕ: Используем полный путь ElectronicsStore.Domain.Enum.StatusCode
            // чтобы избежать конфликта с методом ControllerBase.StatusCode()
            if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
            {
                // 1. Фильтрация по Категории
                if (!string.IsNullOrEmpty(category))
                {
                    if (category == "tvs")
                        products = products.Where(p => p.Category != null && (p.Category.Name.Contains("Телевизор") || p.Category.Name.Contains("Монитор")));
                    else if (category == "pc")
                        products = products.Where(p => p.Category != null && (p.Category.Name.Contains("ПК") || p.Category.Name.Contains("Ноутбук")));
                    else
                        products = products.Where(p => p.Category != null && p.Category.Name == category);
                }

                // 2. Поиск
                if (!string.IsNullOrEmpty(searchString))
                {
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