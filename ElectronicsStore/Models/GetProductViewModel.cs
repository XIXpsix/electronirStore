using ElectronicsStore.Domain; // Для доступа к сущности Product, если нужно, или дублируйте поля

namespace ElectronicsStore.Models
{
    public class GetProductViewModel
    {
        public int Id { get; set; }
        // Инициализируем свойства, чтобы не было предупреждений
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;

        public List<string> GalleryImages { get; set; } = new List<string>();
    }
}