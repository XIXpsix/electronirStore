using Microsoft.AspNetCore.Http; // Исправляет ошибку с IFormFile
using System;
using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Domain.ViewModels
{
    public class UserProfileViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Имя пользователя")]
        [Required(ErrorMessage = "Укажите имя")]
        [MinLength(3, ErrorMessage = "Имя должно быть длиннее 3 символов")]
        public string Name { get; set; } = string.Empty; // Инициализация (исправляет ошибку NULL)

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Загрузить аватар")]
        public IFormFile? Avatar { get; set; } // IFormFile теперь найден

        public string? CurrentAvatarPath { get; set; }
    }
}