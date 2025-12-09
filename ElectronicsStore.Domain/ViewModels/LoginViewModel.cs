using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Domain.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите Email")]
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; } = string.Empty; // Добавили = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; // Добавили = string.Empty;
    }
}