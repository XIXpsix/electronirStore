using System;

namespace ElectronicsStore.Domain.Entity
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public string Address { get; set; } = string.Empty; // Адрес доставки

        public decimal TotalPrice { get; set; } // Сумма заказа

        // Связь с пользователем
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}