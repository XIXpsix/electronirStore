using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ElectronicsStore.Domain.ViewModels;

namespace ElectronicsStore.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        // Если у вас есть конструктор с сервисами - оставьте его.
        // Если нет, вот стандартный вариант:

        [HttpGet]
        public IActionResult Profile() // УБРАЛИ async Task<>
        {
            // Пока просто возвращаем представление
            return View();
        }

        [HttpPost]
        public IActionResult EditProfile(UserProfileViewModel model) // УБРАЛИ async Task<>
        {
            if (ModelState.IsValid)
            {
                // Логика сохранения будет позже
                return RedirectToAction("Profile");
            }
            return View("Profile", model);
        }
    }
}