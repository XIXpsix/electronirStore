using ElectronicsStore.Domain.Enum; // Убедитесь, что этот namespace существует
using System;
using System.Collections.Generic;
using System.Data;

namespace ElectronicsStore.Domain.Entity
{
    public class User
    {
        public Guid Id { get; set; }

        // Код в сервисах ищет Name, поэтому добавим его или переименуем Login
        public string Name { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Исправляем тип string на Role (Enum), чтобы работать с ролями как с объектами
        public Role Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Если у вас есть система отзывов
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}