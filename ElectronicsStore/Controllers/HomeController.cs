using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.ViewModels; // Теперь это пространство имен существует
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;

namespace ElectronicsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService; // Добавляем сервис категорий

        // Обновляем конструктор
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
        public async Task<IActionResult> Catalog()
        {
            // 1. Получаем товары
            var productsResponse = await _productService.GetProducts();

            // 2. Получаем категории (ИСПРАВЛЕНО ИМЯ МЕТОДА)
            var categoriesResponse = await _categoryService.GetAllCategories(); // Было GetCategories

            // 3. Собираем ViewModel
            var model = new CatalogViewModel
            {
                Products = productsResponse.Data ?? new List<Domain.Entity.Product>(),
                Categories = categoriesResponse.Data ?? new List<Domain.Entity.Category>()
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}