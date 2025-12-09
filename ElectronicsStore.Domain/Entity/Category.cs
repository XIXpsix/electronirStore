using System.Collections.Generic;


namespace ElectronicsStore.Domain.Entity
{
    public class Category
    {
        public int Id { get; set; }

        // Инициализируем пустой строкой, чтобы убрать ошибку NULL
        public string Name { get; set; } = string.Empty;

        // Добавил Slug, так как ругался компилятор
        public string Slug { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImagePath { get; set; } = string.Empty;

        // Инициализируем список сразу
        public List<Product> Products { get; set; } = new List<Product>();
    }
}