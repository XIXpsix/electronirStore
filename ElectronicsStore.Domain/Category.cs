using System.Collections.Generic;

namespace ElectronicsStore.Domain
{
    public class Category
    {
        public int Id { get; set; }

        // Название категории (например, "Смартфоны")
        public string Name { get; set; } = string.Empty;

        // ЧПУ (URL-адрес, например, "smartphones"), пригодится для SEO
        public string Slug { get; set; } = string.Empty;

        // Путь к картинке (обязательно добавьте это поле!)
        public string ImagePath { get; set; } = string.Empty;

        // Описание категории
        public string Description { get; set; } = string.Empty;

        // Связь с товарами (одна категория -> много товаров)
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}