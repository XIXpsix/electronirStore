using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Filters;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElectronicsStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> List(int categoryId)
        {
            var response = await _productService.GetProductsByCategory(categoryId);

            // Используем ПОЛНОЕ имя перечисления, чтобы избежать ошибки "ControllerBase.StatusCode"
            if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK && response.Data != null)
            {
                return View(response.Data);
            }

            return RedirectToAction("Error", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            var response = await _productService.GetProduct(id);

            if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK && response.Data != null)
            {
                return View(response.Data);
            }

            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Filter([FromBody] ProductFilter filter)
        {
            var response = await _productService.GetProductsByFilter(filter);

            if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
            {
                return Json(new { data = response.Data });
            }

            return Json(new { error = response.Description });
        }
    }
}