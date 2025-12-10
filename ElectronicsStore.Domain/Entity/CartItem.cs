using System;

namespace ElectronicsStore.Domain.Entity
{
    public class CartItem
    {
        public Guid Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!; // Гарантируем, что не null

        public Guid CartId { get; set; }
        public Cart Cart { get; set; } = null!; // Гарантируем, что не null
    }
}