using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicsStore.Domain.Entity
{
    public class CartItem
    {
        public int Id { get; set; }

        // Связь с пользователем
        public Guid UserId { get; set; }
        // Добавлен '?', так как связанный объект может быть null, если не загружен
        [ForeignKey("UserId")]
        public User? User { get; set; }

        // Связь с товаром
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        public int Quantity { get; set; } = 1;
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}