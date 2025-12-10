using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity; // Для модели Product
using ElectronicsStore.Domain.ViewModels; // Для ErrorViewModel
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq; // Для LINQ-запросов (Where, ToList)
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace ElectronicsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // Внедрение сервиса товаров для доступа к данным
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        // --- Главные страницы (чистые) ---
        public IActionResult Index()
        {
            return View();
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
        // ---------------------------------

        // --- Страница Каталога с логикой поиска ---
        public async Task<IActionResult> Catalog(string searchString)
        {
            // Получаем все товары через сервис
            var response = await _productService.GetProducts();

            if (response.StatusCode != Domain.Enum.StatusCode.OK || response.Data == null)
            {
                // В случае ошибки загрузки
                ModelState.AddModelError("", response.Description ?? "Не удалось загрузить каталог товаров.");
                return View(new List<Product>());
            }

            var products = response.Data.ToList();

            // Логика фильтрации по поисковой строке
            if (!string.IsNullOrEmpty(searchString))
            {
                // Сохраняем строку поиска для отображения в поле
                ViewData["CurrentFilter"] = searchString;

                // Фильтруем товары по названию (без учета регистра)
                products = products
                    .Where(p => p.Name != null && p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Передаем отфильтрованный или полный список в представление
            return View(products);
        }

        // --- Страница Ошибок ---
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}