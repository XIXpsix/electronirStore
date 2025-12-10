using System;
using System.Collections.Generic;
using ElectronicsStore.Domain.Enum;

namespace ElectronicsStore.Domain.Entity
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Role Role { get; set; } = Role.User;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- НОВЫЕ ПОЛЯ ---
        public string ConfirmationCode { get; set; } = string.Empty; // Код из письма
        public bool IsEmailConfirmed { get; set; } = false;          // Подтвержден ли
        // -------------------

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}