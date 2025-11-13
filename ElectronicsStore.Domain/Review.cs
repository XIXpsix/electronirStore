using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicsStore.Domain
{
    public class Review
    {
        public int Id { get; set; } // Первичный ключ
        public int ProductId { get; set; } // Внешний ключ: ID товара, к которому относится отзыв
        public string Author { get; set; } = string.Empty; // Имя автора отзыва
        public int Rating { get; set; } // Оценка (от 1 до 5)
        public string Text { get; set; } = string.Empty; // Текст отзыва

        // Навигационное свойство
        public Product Product { get; set; } = null!; // Связанный товар (1 к 1)
    }
}
