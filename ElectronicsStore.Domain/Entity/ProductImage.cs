using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Domain.Entity
{
    public class ProductImage
    {
        public int Id { get; set; }

        // Знак вопроса ? разрешает значение NULL
        public string? ImagePath { get; set; }

        public int ProductId { get; set; }

        // Знак вопроса ? разрешает значение NULL
        public Product? Product { get; set; }
    }
}