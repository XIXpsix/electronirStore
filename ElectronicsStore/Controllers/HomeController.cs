using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Filters;
using ElectronicsStore.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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

        // Главная страница теперь перенаправляет на Каталог
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Catalog");
        }

        [HttpGet]
        public async Task<IActionResult> Catalog(ProductFilter filter)
        {
            var model = new CatalogViewModel();

            // Логика определения страницы:
            // Если категория не выбрана (0) И нет поискового запроса -> Показываем "Главную страницу каталога" (выбор категорий)
            if (filter.CategoryId == 0 && string.IsNullOrWhiteSpace(filter.Name))
            {
                model.IsMainCatalogPage = true;

                // Получаем список всех категорий для отображения карточек
                var categoriesResponse = await _categoryService.GetCategories();
                model.Categories = categoriesResponse.Data;
            }
            else
            {
                // Иначе показываем "Список товаров" (внутри категории или результаты поиска)
                model.IsMainCatalogPage = false;

                // 1. Получаем товары по фильтру
                var productsResponse = await _productService.GetProductsByFilter(filter);

                // 2. Получаем категории для выпадающего списка в фильтре
                var categoriesResponse = await _categoryService.GetCategories();

                model.Products = productsResponse.Data;
                model.Categories = categoriesResponse.Data;
                model.Filter = filter;

                // 3. Формируем заголовок страницы
                if (filter.CategoryId > 0)
                {
                    // Если выбрана категория, ищем её название
                    var catName = model.Categories.FirstOrDefault(c => c.Id == filter.CategoryId)?.Name;
                    model.CurrentCategoryName = catName ?? "Категория";
                }
                else
                {
                    // Если это просто поиск по всем товарам
                    model.CurrentCategoryName = $"Поиск: {filter.Name}";
                }
            }

            return View(model);
        }

        // Метод для AJAX-фильтрации (обновляет только сетку товаров без перезагрузки)
        [HttpPost]
        public async Task<IActionResult> Filter([FromBody] ProductFilter filter)
        {
            var response = await _productService.GetProductsByFilter(filter);

            // Возвращаем частичное представление (Partial View)
            return PartialView("_ProductListPartial", response.Data);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}