using ElectronicsStore.Domain.Entity;
using System;

// БЫЛО: namespace ElectronicsStore.Domain.Entity (или другое)
// СТАЛО:
namespace ElectronicsStore.Domain
{
    public class Review
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public string Text { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}