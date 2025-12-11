using System;
using System.Collections.Generic;

namespace ElectronicsStore.Domain.Entity
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }

        // Поля, необходимые для BLL
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Связи
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Инициализацию коллекции можно упростить.
        public List<ProductImage> Images { get; set; } = new();
        public List<Review> Reviews { get; set; } = new();
        public string ImagePath { get; set; } = string.Empty; // ИСПРАВЛЕНО: Инициализация для устранения NRT-предупреждения
    }
}