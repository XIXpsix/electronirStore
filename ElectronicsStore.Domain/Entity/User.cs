using System;
using System.Collections.Generic;

namespace ElectronicsStore.Domain.Entity
{
    public class User
    {
        public Guid Id { get; set; }

        // Инициализация = ""; убирает ошибки "не допускает NULL"
        public string Login { get; set; } = "";

        public string Password { get; set; } = ""; // Здесь будет хеш пароля

        public string Email { get; set; } = "";

        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Связь с отзывами
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}