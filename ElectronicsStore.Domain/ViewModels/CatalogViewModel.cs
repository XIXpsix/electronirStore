using ElectronicsStore.Domain.Entity;
using System.Collections.Generic;

namespace ElectronicsStore.Domain.ViewModels
{
    public class CatalogViewModel
    {
        // Инициализация коллекций для устранения NRT
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();

        // Свойства для сохранения текущего состояния фильтров 
        public int CurrentCategoryId { get; set; }
        public string? CurrentSearchName { get; set; }
    }
}