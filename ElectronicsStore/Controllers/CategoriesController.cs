using ElectronicsStore.Domain;
using ElectronicsStore.Domain.Entity;
using Microsoft.AspNetCore.Mvc;
using ElectronicsStore.BLL.Interfaces;
using System.Threading.Tasks;

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
            var response = await _categoryService.GetCategories();
            if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}