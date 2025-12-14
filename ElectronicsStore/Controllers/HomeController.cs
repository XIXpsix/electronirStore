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

        // ВАЖНО: Конструктор должен принимать ОБА сервиса
        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _productService.GetProducts();

            // Исправление warning: "Возможно, аргумент-ссылка... NULL"
            // Если Data придет null, мы подставим пустой список, чтобы Take(3) не упал
            var products = response.Data ?? new List<Product>();

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View(products.Take(3).ToList());
            }
            return View(new List<Product>());
        }

        [HttpGet]
        public async Task<IActionResult> Catalog(ProductFilter filter)
        {
            // 1. Получаем категории (безопасно)
            var categoriesResponse = await _categoryService.GetCategories();
            var categories = categoriesResponse.Data ?? new List<Category>();

            // 2. Определяем, главная ли это страница (нет фильтров)
            bool isMainPage = filter.CategoryId == 0 && string.IsNullOrWhiteSpace(filter.Name);

            // 3. Подготавливаем переменные
            IEnumerable<Product> products = new List<Product>();
            string categoryName = "Каталог";

            // 4. Если нужно искать товары
            if (!isMainPage)
            {
                var productsResponse = await _productService.GetProductsByFilter(filter);
                products = productsResponse.Data ?? new List<Product>();

                if (filter.CategoryId > 0)
                {
                    var cat = categories.FirstOrDefault(c => c.Id == filter.CategoryId);
                    categoryName = cat?.Name ?? "Категория";
                }
                else
                {
                    categoryName = $"Поиск: {filter.Name}";
                }
            }

            // 5. Создаем модель, инициализируя ВСЕ свойства
            var model = new CatalogViewModel
            {
                IsMainCatalogPage = isMainPage,
                Categories = categories,
                Products = products,
                Filter = filter,
                CurrentCategoryName = categoryName,
                // Заполняем обязательное поле, на которое ругался компилятор
                CurrentSearchName = filter.Name ?? string.Empty
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Filter([FromBody] ProductFilter filter)
        {
            var response = await _productService.GetProductsByFilter(filter);
            // Безопасная передача данных в PartialView
            return PartialView("_ProductListPartial", response.Data ?? new List<Product>());
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