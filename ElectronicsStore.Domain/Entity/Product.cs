using System.Collections.Generic; // Нужно для List
using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Domain.Entity
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // Исправляем предупреждение NULL

        [Required]
        public string Description { get; set; } = string.Empty; // Исправляем предупреждение NULL

        public decimal Price { get; set; }

        // Вместо ImagePath мы используем Avatar (картинка в байтах), как договаривались ранее
        public byte[]? Avatar { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!; // Исправляем предупреждение NULL

        // Добавляем список отзывов, которого не хватало
        public List<Review> Reviews { get; set; } = new List<Review>();
        public required string ImagePath { get; set; }
    }
}