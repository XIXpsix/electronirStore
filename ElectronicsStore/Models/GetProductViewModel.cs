using ElectronicsStore.Domain.Entity;
using System.Collections.Generic;

namespace ElectronicsStore.Models
{
    public class GetProductViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        // --- ВОТ ЭТИ ПОЛЯ, КОТОРЫХ НЕ ХВАТАЛО ---
        public string ImageUrl { get; set; } = "/img/w.png";

        public double AverageRating { get; set; }

        public int ReviewsCount { get; set; }

        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}