using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Filters;
using System.Collections.Generic;

namespace ElectronicsStore.Domain.ViewModels
{
    public class CatalogViewModel
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();

        // Список категорий для отображения в фильтре
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();

        // Текущее состояние фильтра (чтобы при обновлении страницы значения не терялись)
        public ProductFilter Filter { get; set; } = new ProductFilter();
        public string CurrentSearchName { get; set; }
        public string CurrentCategoryName { get; set; }
        public bool IsMainCatalogPage { get; set; }
    }
}