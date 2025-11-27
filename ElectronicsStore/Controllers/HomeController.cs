using ElectronicsStore.DAL;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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

        // ✅ ВОТ ТУТ МЫ ОТКАТИЛИ ИЗМЕНЕНИЯ
        public IActionResult Catalog()
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