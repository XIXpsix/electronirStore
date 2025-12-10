using System;
using System.Collections.Generic;

namespace ElectronicsStore.Domain.Entity
{
    public class Cart
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public List<CartItem> Items { get; set; } = new();
    }
}