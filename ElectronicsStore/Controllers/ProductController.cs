using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Filters; // Обязательно подключаем фильтры
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
            ViewBag.CategoryId = categoryId; // Это важно для работы JS фильтра
            var response = await _productService.GetProductsByCategory(categoryId);

            if (response.StatusCode != ElectronicsStore.Domain.Enum.StatusCode.OK)
            {
                // Обработка ошибки
                return Content($"Ошибка: {response.Description}");
            }

            // Возвращает Views/Product/List.cshtml, где есть фильтры
            return View(response.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            // 1. Получаем сам товар
            var productResponse = await _productService.GetProduct(id);

            // 2. Получаем дополнительные картинки
            var imagesResponse = await _productService.GetImagesByProductId(id);

            if (productResponse.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
            {
                var product = productResponse.Data;

                // 3. Создаем ViewModel и заполняем данными
                var model = new GetProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    ImagePath = product.ImagePath,
                    CategoryName = product.Category?.Name ?? "Без категории",
                    GalleryImages = imagesResponse.Data ?? new List<string>()
                };

                return View(model);
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