using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.ViewModels; // Обязательно для CatalogViewModel
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _productService.GetProducts();
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                // На главной показываем топ-3 товара
                return View(response.Data.Take(3).ToList());
            }
            return View(new List<Product>());
        }

        [HttpGet]
        public async Task<IActionResult> Catalog(string category, string searchString)
        {
            // 1. Создаем модель, которую ждет View (CatalogViewModel)
            // Это решит ошибку "InvalidOperationException... requires CatalogViewModel"
            var model = new CatalogViewModel
            {
                Products = new List<Product>(),
                CurrentSearchName = searchString,
                // Если нужно, можно добавить свойство CurrentCategorySlug в ViewModel и заполнить его
            };

            var response = await _productService.GetProducts();

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                var products = response.Data;

                // 2. Фильтрация по ПОИСКУ (если введен текст)
                if (!string.IsNullOrEmpty(searchString))
                {
                    products = products.Where(p => p.Name.ToLower().Contains(searchString.ToLower())).ToList();
                }

                // 3. Фильтрация по КАТЕГОРИИ (ИСПРАВЛЕНО)
                // Если категория выбрана и это не "все товары"
                if (!string.IsNullOrEmpty(category) && category != "all")
                {
                    // Мы проверяем, совпадает ли Slug категории товара с тем, что пришло в ссылке.
                    // Например: category="smartphones" -> ищем товары, у которых Category.Slug == "smartphones"
                    products = products.Where(p =>
                        p.Category != null &&
                        string.Equals(p.Category.Slug, category, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                model.Products = products;
            }

            return View(model);
        }

        public IActionResult About() => View();
        public IActionResult Contacts() => View();
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}