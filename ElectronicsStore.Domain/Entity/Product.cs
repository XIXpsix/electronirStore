using System.Collections.Generic;

// ВАЖНО: Пространство имен должно заканчиваться на .Entity
namespace ElectronicsStore.Domain.Entity
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImagePath { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        // Упрощенная инициализация (убирает предупреждение)
        public ICollection<Review> Reviews { get; set; } = [];
    }
}