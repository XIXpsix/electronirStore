using ElectronicsStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ElectronicsStore.Controllers
{
    // РЕШЕНИЕ: Вот так выглядит правильный основной конструктор
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        // Поле _logger теперь инициализируется из параметра logger
        private readonly ILogger<HomeController> _logger = logger;

        public IActionResult Index()
        {
            // Убедись, что у тебя нет второго public IActionResult Index()
            return View();
        }

        public IActionResult Privacy()
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
