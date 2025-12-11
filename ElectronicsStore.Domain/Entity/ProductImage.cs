using System;

namespace ElectronicsStore.Domain.Entity
{
    public class ProductImage
    {
        public int Id { get; set; }
        // FIX: Явная инициализация для устранения ошибки конструктора
        // Примечание: Использование 'required' на ImagePath - это правильное решение для устранения предупреждения.

        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public required string ImagePath { get; set; } // Оставлено 'required', что решает проблему NRT
    }
}