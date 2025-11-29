using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите почту или логин")]
        public string LoginOrEmail { get; set; } = string.Empty; // ✅ Исправлено

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty; // ✅ Исправлено

        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; } // ? означает, что здесь МОЖЕТ быть null
    }
}