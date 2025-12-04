using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicsStore.Domain.Filters
{
    public class ProductFilter
    {
        public int CategoryId { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }

        // Инициализируем значением по умолчанию
        public string SortType { get; set; } = "price_asc";
    }
}