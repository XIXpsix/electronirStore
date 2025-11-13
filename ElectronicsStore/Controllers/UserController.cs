using Microsoft.AspNetCore.Mvc;

namespace ElectronicsStore.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Profile()
        {
            return View();
        }
    }
}
