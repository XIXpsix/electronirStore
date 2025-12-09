using System.Collections.Generic;

namespace ElectronicsStore.Domain.Entity
{
    public class Category
    {
        public int Id { get; set; }

        // »спользуем required, чтобы гарантировать заполнение, 
        // или оставл€ем инициализацию = string.Empty;
        public required string Name { get; set; }
        public required string Slug { get; set; }
        public string ImagePath { get; set; } = string.Empty; // ћожно оставить так
        public string Description { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}