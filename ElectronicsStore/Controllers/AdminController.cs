// ПОЛНЫЙ КОД
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ElectronicsStore.BLL.Interfaces;
using System.Threading.Tasks;
using ElectronicsStore.Domain.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using ElectronicsStore.Domain.Entity;
using System.Collections.Generic;

namespace ElectronicsStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _appEnvironment;

        public AdminController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment appEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _appEnvironment = appEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _productService.GetProducts();
            if (response.StatusCode == Domain.Enum.StatusCode.OK && response.Data != null)
            {
                return View(response.Data.ToList());
            }
            return View(new List<Product>());
        }

        // --- СОЗДАНИЕ ТОВАРА ---

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesDropdown();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.CreateProduct(model);

                if (response.StatusCode == Domain.Enum.StatusCode.OK && response.Data != null)
                {
                    if (model.NewImages != null)
                    {
                        await SaveImages(response.Data.Id, model.NewImages);
                    }

                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", response.Description ?? "Неизвестная ошибка при создании товара.");
            }
            await PopulateCategoriesDropdown();
            return View(model);
        }

        // --- РЕДАКТИРОВАНИЕ ТОВАРА ---

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _productService.GetProductForEdit(id);
            if (response.StatusCode == Domain.Enum.StatusCode.OK && response.Data != null)
            {
                await PopulateCategoriesDropdown();
                return View(response.Data);
            }
            TempData["ErrorMessage"] = response.Description ?? "Товар не найден.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _productService.EditProduct(model);

                if (response.StatusCode == Domain.Enum.StatusCode.OK && response.Data != null)
                {
                    if (model.NewImages != null)
                    {
                        await SaveImages(model.Id, model.NewImages);
                    }

                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", response.Description ?? "Неизвестная ошибка при редактировании товара.");
            }
            await PopulateCategoriesDropdown();
            return View(model);
        }

        // --- УДАЛЕНИЕ ТОВАРА ---

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _productService.DeleteProduct(id);

            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                // TODO: Добавить логику для удаления физических файлов изображений с диска
                TempData["SuccessMessage"] = $"Товар ID {id} успешно удален.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Ошибка при удалении: {response.Description}";
            }

            return RedirectToAction("Index");
        }

        // --- УДАЛЕНИЕ ИЗОБРАЖЕНИЯ (AJAX) ---

        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var response = await _productService.DeleteImage(imageId);
            if (response.StatusCode == Domain.Enum.StatusCode.OK)
            {
                // TODO: Добавить логику для удаления физического файла с диска
                return Json(new { success = true, message = "Изображение удалено." });
            }
            return Json(new { success = false, message = response.Description ?? "Неизвестная ошибка." });
        }

        // --- ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ---

        private async Task PopulateCategoriesDropdown()
        {
            // ИСПРАВЛЕНИЕ: Используем GetCategories()
            var categoriesResponse = await _categoryService.GetCategories();
            var categories = categoriesResponse.Data?
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList() ?? new List<SelectListItem>();

            ViewBag.Categories = categories;
        }

        private async Task SaveImages(int productId, List<IFormFile> images)
        {
            if (images == null || images.Count == 0) return;

            var productsPath = Path.Combine(_appEnvironment.WebRootPath, "images", "products");
            if (!Directory.Exists(productsPath))
            {
                Directory.CreateDirectory(productsPath);
            }

            foreach (var image in images)
            {
                if (image.Length > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string fullPath = Path.Combine(productsPath, fileName);
                    string relativePath = "/images/products/" + fileName;

                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    await _productService.AddImage(productId, relativePath);
                }
            }
        }
    }
}