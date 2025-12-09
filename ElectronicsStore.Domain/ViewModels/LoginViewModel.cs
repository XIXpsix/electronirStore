using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Domain.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите имя")]
        public string Name { get; set; } = string.Empty; // Инициализация

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; // Инициализация
    }
}