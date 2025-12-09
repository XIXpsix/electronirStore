namespace ElectronicsStore.Domain.Entity
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string? ImagePath { get; set; } // Добавили вопросительный знак '?'
        public int ProductId { get; set; }
        public Product? Product { get; set; } // Добавили вопросительный знак '?'
    }
}