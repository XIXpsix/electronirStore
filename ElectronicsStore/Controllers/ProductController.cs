using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

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
        public async Task<IActionResult> GetProduct(int id)
        {
            var response = await _productService.GetProduct(id);

            // ИСПРАВЛЕНИЕ: Проверка статуса и наличия данных
            if (response.StatusCode == Domain.Enum.StatusCode.OK && response.Data != null)
            {
                var product = response.Data;

                // 1. Считаем рейтинг (проверка на null для списка отзывов)
                double avgRating = 0;
                if (product.Reviews != null && product.Reviews.Any())
                {
                    avgRating = product.Reviews.Average(r => r.Rating);
                }

                // 2. Получаем картинку (безопасная проверка на null)
                string imageUrl = "/img/w.png";

                // Проверяем список Images на null перед обращением
                if (product.Images != null && product.Images.Any())
                {
                    var firstImg = product.Images.FirstOrDefault();
                    // firstImg может быть null, проверяем это
                    if (firstImg != null && !string.IsNullOrEmpty(firstImg.ImagePath))
                    {
                        imageUrl = firstImg.ImagePath;
                    }
                }
                // Запасной вариант
                else if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    imageUrl = product.ImagePath;
                }

                // 3. Создаем ViewModel с защитой от null
                var viewModel = new GetProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name ?? "Без названия",
                    Description = product.Description ?? "Описание отсутствует",
                    Price = product.Price,
                    // Безопасное получение имени категории (CS8602)
                    CategoryName = product.Category?.Name ?? "Без категории",

                    ImageUrl = imageUrl,

                    // Безопасная сортировка отзывов
                    Reviews = product.Reviews != null
                        ? product.Reviews.OrderByDescending(r => r.CreatedAt).ToList()
                        : new List<Review>(),

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
            // ИСПРАВЛЕНИЕ: Безопасное получение имени
            var userName = User.Identity?.Name ?? "";

            var response = await _productService.AddReview(userName, productId, content, rating);

            // ... остальной код ...
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                return RedirectToAction("GetProduct", new { id = productId });
            }
            return RedirectToAction("GetProduct", new { id = productId });
        }
    }
}