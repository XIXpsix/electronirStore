using ElectronicsStore.BLL.Interfaces; // Подключаем интерфейс из BLL
using ElectronicsStore.Domain;
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

            // Если успех — показываем список товаров
            if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
            {
                return View(response.Data);
            }

            return RedirectToAction("Error", "Home");
        }
    }
}