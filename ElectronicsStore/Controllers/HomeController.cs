using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ElectronicsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Index() => View();
        public IActionResult Privacy() => View();
        public IActionResult About() => View();
        public IActionResult Contacts() => View();

        [HttpGet]
        public async Task<IActionResult> Catalog(string category, string searchString)
        {
            var response = await _productService.GetProducts();
            IEnumerable<Product> products = response.Data ?? [];

            if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
            {
                if (!string.IsNullOrEmpty(category))
                {
                    // ИСПРАВЛЕНИЕ: Проверяем p.Category?.Name != null
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