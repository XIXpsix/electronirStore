using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Domain.Entity
{
    public class ProductImage
    {
            public int Id { get; set; }
            public int ProductId { get; set; }
            // Инициализируем пустой строкой
            public string ImagePath { get; set; } = string.Empty;
    }
}