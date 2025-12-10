namespace ElectronicsStore.Domain.Entity
{
    public class ProductImage
    {
        public int Id { get; set; }

        // —сылка на путь картинки
        public string? ImagePath { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}