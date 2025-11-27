using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ElectronicsStore.Domain.Entity
{
    public class ApplicationUser : IdentityUser
    {
        // Инициализируем пустой строкой, чтобы ошибка исчезла
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // Связь с отзывами (чтобы не было ошибки в базе данных)
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}