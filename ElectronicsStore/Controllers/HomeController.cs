using ElectronicsStore.Domain.ViewModels;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ElectronicsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // --- ДОБАВЛЕНО ---
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contacts()
        {
            return View();
        }

        public IActionResult Catalog()
        {
            // Если у вас есть контроллер Product, можно сделать return RedirectToAction("List", "Product");
            // Но пока просто вернем представление
            return View();
        }
        // -----------------

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}