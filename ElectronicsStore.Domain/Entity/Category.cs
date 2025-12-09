using System.Collections.Generic;

namespace ElectronicsStore.Domain.Entity
{
    public class Category
    {
        public int Id { get; set; }

        // Используем required, чтобы гарантировать заполнение, 
        // или оставляем инициализацию = string.Empty;
        public required string Name { get; set; }
        public required string Slug { get; set; }
        public required string ImagePath { get; set; }
        public string Description { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}