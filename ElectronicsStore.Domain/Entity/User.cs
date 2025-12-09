using ElectronicsStore.Domain.Entity;
using System;
using System.Collections.Generic;

// БЫЛО: namespace ElectronicsStore.Domain.Entity
// СТАЛО:
namespace ElectronicsStore.Domain
{
    public class User
    {
        public Guid Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Связь с отзывами
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}