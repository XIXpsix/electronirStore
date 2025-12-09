using ElectronicsStore.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ElectronicsStore.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(ILogger<HomeController> logger)
        {
        }

        public IActionResult Index() => View();
        public IActionResult About() => View();

        [HttpGet]
        public IActionResult Contacts() => View();

        [HttpPost]
        public IActionResult Contacts(string name, string email, string message)
        {
            ViewBag.Message = "Спасибо за ваше сообщение!";
            return View();
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // ✅ ИСПРАВЛЕНИЕ: Инициализация с пустой строкой вместо null
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            return View(new ErrorViewModel
            {
                RequestId = requestId ?? string.Empty
            });
        }
    }
}