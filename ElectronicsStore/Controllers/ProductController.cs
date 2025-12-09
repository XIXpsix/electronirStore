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

            // Проверяем статус И наличие данных
            if (productResponse.StatusCode == Domain.Enum.StatusCode.OK && productResponse.Data != null)
            {
                // ВАЖНО: Мы передаем в View сам объект Product (productResponse.Data),
                // так как в GetProduct.cshtml указано @model ElectronicsStore.Domain.Entity.Product
                return View(productResponse.Data);
            }

            // Если товар не найден
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