using System;
using System.Collections.Generic;

namespace ElectronicsStore.Domain.Entity
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Исправлено: namespace и инициализация
        public ICollection<Review> Reviews { get; set; } = [];
    }
}