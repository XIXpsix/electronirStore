using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity;
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

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            // На главной тоже можно показать пару товаров (например, топ-3)
            var response = await _productService.GetProducts();
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View(response.Data.Take(3).ToList());
            }
            return View(new List<Product>());
        }

        // ЭТОТ МЕТОД ОТВЕЧАЕТ ЗА СТРАНИЦУ КАТАЛОГА
        [HttpGet]
        public async Task<IActionResult> Catalog(string searchString)
        {
            var response = await _productService.GetProducts();

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                var products = response.Data;

                // Если в поиске что-то написали, фильтруем
                if (!string.IsNullOrEmpty(searchString))
                {
                    products = products.Where(p => p.Name.ToLower().Contains(searchString.ToLower())).ToList();
                }

                return View(products);
            }

            return View(new List<Product>());
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