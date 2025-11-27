using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Models
{
    public class LoginViewModel
    {
        // ✅ Теперь это универсальное поле
        [Required(ErrorMessage = "Введите Email или Логин")]
        [Display(Name = "Email или Логин")]
        public string LoginOrEmail { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}