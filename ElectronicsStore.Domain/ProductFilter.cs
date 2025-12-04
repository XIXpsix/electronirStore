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

        // Тип сортировки: "price_asc", "price_desc", "name_asc"
        public string SortType { get; set; }
    }
}