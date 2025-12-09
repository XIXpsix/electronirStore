using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Filters;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

            // Проверяем статус И наличие данных
            if (response.StatusCode == Domain.Enum.StatusCode.OK && response.Data != null)
            {
                return View(response.Data);
            }

            // Если данных нет или ошибка
            return RedirectToAction("Error");
        }

        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            var productResponse = await _productService.GetProduct(id);
            var imagesResponse = await _productService.GetImagesByProductId(id);

            if (productResponse.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK && productResponse.Data != null)
            {
                var product = productResponse.Data;

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
                return Json(new { data = response.Data });
            }

            return Json(new { error = response.Description });
        }
    }
}