using ElectronicsStore.DAL; // или BLL, если используешь сервисы
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Нужно для .Include и .ToListAsync
using System.Diagnostics;
using System.Linq;



namespace ElectronicsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ElectronicsStoreContext _context;

        public HomeController(ILogger<HomeController> logger, ElectronicsStoreContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index() => View();
        public IActionResult About() => View();
        public IActionResult Contacts() => View();
        public IActionResult Privacy() => View();

        public IActionResult Catalog()
        {
            return View();
        }
        // Метод для показа товаров по категории
        [HttpGet]
        public async Task<IActionResult> ProductsByCategory(int id)
        {
            // Ищем товары, у которых CategoryId совпадает с тем, что мы нажали
            var products = await _context.Products
                .Where(p => p.CategoryId == id)
                .Include(p => p.Category)
                .ToListAsync();

            if (!products.Any())
            {
                // Если товаров нет, можно вернуть пустую View или сообщение
                return View("Catalog"); // Пока просто вернем в каталог
            }

            // Создадим новую View (страницу) для списка товаров
            return View("ProductList", products);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}