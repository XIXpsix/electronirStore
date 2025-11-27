using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите Email")]
        [EmailAddress(ErrorMessage = "Некорректный Email")]
        public string Email { get; set; }

        // ✅ НОВОЕ ПОЛЕ: Логин (вместо Имени и Фамилии)
        [Required(ErrorMessage = "Придумайте логин")]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Придумайте пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Повторите пароль")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }
    }
}