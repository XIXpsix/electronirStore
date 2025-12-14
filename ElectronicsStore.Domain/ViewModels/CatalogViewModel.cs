using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Filters;
using System.Collections.Generic;

namespace ElectronicsStore.Domain.ViewModels
{
    public class CatalogViewModel
    {
        // Списки инициализируем пустыми, чтобы не было NullReference
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();

        public ProductFilter Filter { get; set; } = new ProductFilter();

        // Задаем значения по умолчанию, чтобы избежать ошибок "Необходимо задать обязательный элемент"
        public string CurrentCategoryName { get; set; } = "Каталог";

        // Добавляем это свойство, так как компилятор на него ругался
        public string CurrentSearchName { get; set; } = string.Empty;

        public bool IsMainCatalogPage { get; set; }
    }
}