using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicsStore.Domain
{
    public class Product
    {
        public int Id { get; set; } // Первичный ключ
        public string Name { get; set; } = string.Empty; // Название товара
        public string Description { get; set; } = string.Empty; // Описание
        public decimal Price { get; set; } // Цена

        // Внешний ключ: указывает, к какой категории принадлежит товар
        public int CategoryId { get; set; }

        // Навигационные свойства (связи с другими таблицами)
        public Category Category { get; set; } = null!; // Связанная категория (1 к 1)
        public ICollection<Review> Reviews { get; set; } = new List<Review>(); // Список отзывов (1 ко многим)
    }
}
