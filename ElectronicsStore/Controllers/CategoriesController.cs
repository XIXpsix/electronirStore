using ElectronicsStore.Domain;
using Microsoft.AspNetCore.Mvc;
// ВАЖНО: Используем BLL, а не Service
using ElectronicsStore.BLL.Interfaces;

namespace ElectronicsStore.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> ListOfCategories()
        {
            var response = await _categoryService.GetAllCategories();

            if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}