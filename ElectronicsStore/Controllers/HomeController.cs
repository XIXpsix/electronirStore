using ElectronicsStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ElectronicsStore.Controllers
{
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;

        // ✅ ЭТО СТРАНИЦА "ГЛАВНАЯ"
        public IActionResult Index()
        {
            return View();
        }

        // ✅ НОВЫЙ МЕТОД ДЛЯ СТРАНИЦЫ "О НАС"
        public IActionResult About()
        {
            return View();
        }

        // ✅ НОВЫЙ МЕТОД ДЛЯ СТРАНИЦЫ "КАТАЛОГ"
        public IActionResult Catalog()
        {
            return View();
        }

        // ✅ НОВЫЙ МЕТОД ДЛЯ СТРАНИЦЫ "КОНТАКТЫ"
        public IActionResult Contacts()
        {
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