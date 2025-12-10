using System;

namespace ElectronicsStore.Domain.Entity
{
    public class ProductImage
    {
        public int Id { get; set; }
        // FIX: явна€ инициализаци€ дл€ устранени€ ошибки конструктора
        

        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public required string ImagePath { get; set; }
    }
}