namespace ElectronicsStore.Domain.Entity
{
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ImagePath { get; set; } = string.Empty;
    }
}