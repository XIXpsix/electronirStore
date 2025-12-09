namespace ElectronicsStore.Domain.Filters
{
    public class ProductFilter
    {
        public int CategoryId { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string SortType { get; set; } = "default";
    }
}