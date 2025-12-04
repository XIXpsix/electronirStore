using ElectronicsStore.Domain; // Для доступа к сущности Product, если нужно, или дублируйте поля

namespace ElectronicsStore.Models
{
    public class GetProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; } // Главная картинка
        public string CategoryName { get; set; }

        // Список дополнительных картинок (Галерея)
        public List<string> GalleryImages { get; set; } = new List<string>();
    }
}