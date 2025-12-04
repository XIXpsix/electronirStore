using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Domain.Entity
{
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; } // Внешний ключ
        public string ImagePath { get; set; } // Путь к картинке

        // Навигационное свойство (опционально, если нужно)
        // public Product Product { get; set; } 
    }
}