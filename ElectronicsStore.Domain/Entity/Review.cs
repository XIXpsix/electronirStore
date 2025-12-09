using System;

namespace ElectronicsStore.Domain.Entity
{
    public class Review
    {
        public int Id { get; set; }

        public string Text { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Связь с товаром
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        // Связь с пользователем
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
    }
}