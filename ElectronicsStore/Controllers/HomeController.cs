using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Filters;
using ElectronicsStore.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        // ✅ ИСПРАВЛЕНИЕ: Удалены неиспользуемые поля (categoryId, name, showAll),
        // которые вызывали ошибки и требовали перезапуска при удалении.

        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _productService.GetProducts();

            // ✅ ИСПРАВЛЕНИЕ: Безопасное получение списка
            var products = response?.Data ?? new List<Product>();

            if (response != null && response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View(products.Take(3).ToList());
            }
            return View(new List<Product>());
        }

        [HttpGet]
        // ✅ ИСПРАВЛЕНИЕ: Добавлен параметр showAll
        public async Task<IActionResult> Catalog(ProductFilter filter, bool showAll = false)
        {
            // 1. Получаем категории (безопасно)
            var categoriesResponse = await _categoryService.GetCategories();
            var categories = categoriesResponse?.Data ?? new List<Category>();

            // Главная страница показывается, только если нет фильтров и не нажато "Все товары".
            bool isMainPage = filter.CategoryId == 0 && string.IsNullOrEmpty(filter.Name) && !showAll;

            // 3. Подготавливаем переменные
            IEnumerable<Product> products = new List<Product>();
            string categoryName = "Каталог";

            if (!isMainPage)
            {
                var productsResponse = await _productService.GetProductsByFilter(filter);
                products = productsResponse?.Data ?? new List<Product>();

                if (filter.CategoryId > 0)
                {
                    var cat = categories.FirstOrDefault(c => c.Id == filter.CategoryId);
                    categoryName = cat?.Name ?? "Категория";
                }
                else if (!string.IsNullOrEmpty(filter.Name))
                {
                    categoryName = $"Поиск: {filter.Name}";
                }
                else
                {
                    categoryName = "Все товары";
                }
            }

            // 5. Создаем модель
            var model = new CatalogViewModel
            {
                IsMainCatalogPage = isMainPage,
                Categories = categories,
                Products = products,
                Filter = filter,
                CurrentCategoryName = categoryName,
                CurrentSearchName = filter.Name ?? string.Empty
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Filter([FromBody] ProductFilter filter)
        {
            var response = await _productService.GetProductsByFilter(filter);
            // ✅ ИСПРАВЛЕНИЕ: Безопасная передача данных
            return PartialView("_ProductListPartial", response?.Data ?? new List<Product>());
        }

        public IActionResult Privacy() => View();
        public IActionResult About() => View();
        public IActionResult Contacts() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}