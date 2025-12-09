namespace ElectronicsStore.Domain.Entity
{
    public class ProductImage
    {
        public int Id { get; set; }

        // Добавили '?' - теперь может быть пустым
        public string? ImagePath { get; set; }

        public int ProductId { get; set; }

        // Добавили '?' - теперь может быть пустым
        public Product? Product { get; set; }
    }
}