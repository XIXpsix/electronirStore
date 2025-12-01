using Microsoft.AspNetCore.Mvc;

namespace ElectronicsStore.Controllers
{
    public class CategoriesController : Controller
    {
        // Действие для отображения списка категорий
        [HttpGet]
        public IActionResult ListOfCategories()
        {
            return View();
        }
    }
}