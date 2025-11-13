using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введите Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }
    }
}
