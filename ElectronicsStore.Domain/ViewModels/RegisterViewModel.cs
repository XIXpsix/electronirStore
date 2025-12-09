using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Domain.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Укажите имя")]
        [MaxLength(20, ErrorMessage = "Имя слишком длинное")]
        [MinLength(3, ErrorMessage = "Имя слишком короткое")]
        public string Name { get; set; } = string.Empty; // Инициализация

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Укажите почту")]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Укажите пароль")]
        [MinLength(6, ErrorMessage = "Пароль слишком короткий")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; } = string.Empty;

    }
}