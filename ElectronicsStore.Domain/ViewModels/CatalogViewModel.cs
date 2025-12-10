using ElectronicsStore.Domain.Entity;
using System.Collections.Generic;

namespace ElectronicsStore.Domain.ViewModels
{
    public class CatalogViewModel
    {
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();

        // Свойства для сохранения текущего состояния фильтров (опционально)
        public int CurrentCategoryId { get; set; }
        public string? CurrentSearchName { get; set; }
    }
}