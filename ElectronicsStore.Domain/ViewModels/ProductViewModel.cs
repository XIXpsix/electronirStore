using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using ElectronicsStore.Domain.Entity;

namespace ElectronicsStore.Domain.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Название")]
        [Required(ErrorMessage = "Введите название товара")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 100 символов")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Введите описание товара")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Укажите цену")]
        [Range(0.01, 1000000.00, ErrorMessage = "Цена должна быть положительной")]
        public decimal Price { get; set; }

        [Display(Name = "Категория")]
        [Required(ErrorMessage = "Выберите категорию")]
        public int CategoryId { get; set; }

        [Display(Name = "Загрузить изображения")]
        public List<IFormFile>? NewImages { get; set; }

        public List<ProductImage>? ExistingImages { get; set; }
    }
}