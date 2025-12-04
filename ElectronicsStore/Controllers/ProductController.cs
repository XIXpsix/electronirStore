using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Filters; // Обязательно подключаем фильтры
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
            // Сохраняем ID категории, чтобы JS знал, что мы фильтруем
            ViewBag.CategoryId = categoryId;

            var response = await _productService.GetProductsByCategory(categoryId);

            if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);
            }

            return RedirectToAction("Error", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            var response = await _productService.GetProduct(id);

            if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);
            }

            return RedirectToAction("Error");
        }

        [HttpPost]
        public async Task<IActionResult> Filter([FromBody] ProductFilter filter)
        {
            var response = await _productService.GetProductsByFilter(filter);

            if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
            {
                // Возвращаем чистые данные (JSON), JS сам перерисует карточки
                return Json(new { data = response.Data });
            }

            return Json(new { error = response.Description });
        }
    }
}