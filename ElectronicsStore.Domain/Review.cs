using ElectronicsStore.Domain.Entity; // Подключаем пространство имен с ApplicationUser
using System;

namespace ElectronicsStore.Domain
{
    public class Review
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty; // Текст отзыва
        public int Rating { get; set; } // Оценка (1-5)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Дата написания

        // Связь с товаром
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        // Связь с пользователем (Добавляем!)
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
}