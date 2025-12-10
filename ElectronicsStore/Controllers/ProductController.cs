using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicsStore.Controllers
{
    // C# 12: Основной конструктор
    public class ProductController(IProductService productService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetProduct(int id)
        {
            var response = await productService.GetProduct(id);

            if (response.StatusCode == Domain.Enum.StatusCode.OK && response.Data != null)
            {
                var product = response.Data;

                double avgRating = 0;
                // Оптимизация: Count > 0 вместо Any()
                if (product.Reviews != null && product.Reviews.Count > 0)
                {
                    avgRating = product.Reviews.Average(r => r.Rating);
                }

                string imageUrl = "/img/w.png";

                if (product.Images != null && product.Images.Count > 0)
                {
                    var firstImg = product.Images.FirstOrDefault();
                    if (firstImg != null && !string.IsNullOrEmpty(firstImg.ImagePath))
                    {
                        imageUrl = firstImg.ImagePath;
                    }
                }
                else if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    imageUrl = product.ImagePath;
                }

                var viewModel = new GetProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name ?? "Без названия",
                    Description = product.Description ?? "Описание отсутствует",
                    Price = product.Price,
                    CategoryName = product.Category?.Name ?? "Без категории",
                    ImageUrl = imageUrl,
                    Reviews = product.Reviews != null
                        ? product.Reviews.OrderByDescending(r => r.CreatedAt).ToList()
                        : [], // Упрощение []
                    AverageRating = Math.Round(avgRating, 1),
                    ReviewsCount = product.Reviews?.Count ?? 0
                };

                return View(viewModel);
            }

            return RedirectToAction("Error", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(int productId, string content, int rating)
        {
            var userName = User?.Identity?.Name ?? string.Empty;

            var response = await productService.AddReview(userName, productId, content, rating);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return RedirectToAction("GetProduct", new { id = productId });
            }
            return RedirectToAction("GetProduct", new { id = productId });
        }
    }
}