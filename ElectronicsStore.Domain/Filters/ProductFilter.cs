namespace ElectronicsStore.Domain.Filters
{
    public class ProductFilter
    {
        // Категория товара (0 - все категории)
        public int CategoryId { get; set; } = 0;

        // Диапазон цен
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Поиск по названию
        public string Name { get; set; } = string.Empty;

        // Сортировка: "price_asc", "price_desc", "name_asc" и т.д.
        public string SortType { get; set; } = string.Empty;
    }
}