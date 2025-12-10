using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicsStore.Domain.Entity
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        // Картинка в байтах (основная)
        public byte[]? Avatar { get; set; }

        // Путь к картинке (для совместимости, если используется в старых вьюхах)
        public string? ImagePath { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // Связь с дополнительными картинками
        public virtual List<ProductImage> Images { get; set; } = new List<ProductImage>();

        // Связь с отзывами
        public virtual List<Review> Reviews { get; set; } = new List<Review>();

        // Вспомогательное свойство для отображения Avatar в img src (base64)
        [NotMapped]
        public string AvatarUrl => Avatar != null && Avatar.Length > 0
            ? $"data:image/jpeg;base64,{System.Convert.ToBase64String(Avatar)}"
            : (ImagePath ?? "/img/w.png"); // Если аватара нет, берем ImagePath или заглушку
    }
}