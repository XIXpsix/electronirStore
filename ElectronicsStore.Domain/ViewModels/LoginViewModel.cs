using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Domain.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите почту или логин")]
        public string LoginOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }

        // Это поле было пропущено, из-за него ошибка "LoginViewModel не содержит ReturnUrl"
        public string? ReturnUrl { get; set; }
    }
}