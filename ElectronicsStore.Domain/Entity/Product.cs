using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectronicsStore.Domain.Entity
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public byte[]? Avatar { get; set; }

        public string? ImagePath { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public virtual List<ProductImage> Images { get; set; } = new List<ProductImage>();

        public virtual List<Review> Reviews { get; set; } = new List<Review>();

        [NotMapped]
        public string AvatarUrl
        {
            get
            {
                // ИСПРАВЛЕНИЕ: Присваиваем локальной переменной для безопасной проверки на null
                var avatar = Avatar;
                return avatar != null && avatar.Length > 0
                    ? $"data:image/jpeg;base64,{System.Convert.ToBase64String(avatar)}"
                    : (ImagePath ?? "/img/w.png");
            }
        }
    }
}