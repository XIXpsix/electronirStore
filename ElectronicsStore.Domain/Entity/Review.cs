using System;

namespace ElectronicsStore.Domain.Entity
{
    public class Review
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; } // Обычно Id пользователя - int, проверьте, если у вас Guid
        public User User { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}