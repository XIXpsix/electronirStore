namespace ElectronicsStore.Domain.Filters
{
    public class ProductFilter
    {
        public int CategoryId { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }

        // Добавили поле для поиска по названию
        public string Name { get; set; } = string.Empty;

        // Тип сортировки (price_asc, price_desc и т.д.)
        public string SortType { get; set; } = string.Empty;
    }
}