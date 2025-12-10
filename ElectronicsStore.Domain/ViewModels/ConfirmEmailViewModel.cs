using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Domain.ViewModels
{
    public class ConfirmEmailViewModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        public string Email { get; set; } = string.Empty; // Инициализация убирает предупреждение NULL

        [Required(ErrorMessage = "Введите код")]
        public string Code { get; set; } = string.Empty; // Инициализация убирает предупреждение NULL
    }
}