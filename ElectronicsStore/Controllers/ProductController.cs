using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain;
using Microsoft.AspNetCore.Mvc;

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

            // Если успех — показываем список товаров
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);
            }

            return RedirectToAction("Error");
        }
    }
}