using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // ✅ ИСПРАВЛЕНО: Раскомментировано и переименовано
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string PasswordConfirm { get; set; } // <-- Теперь это поле ЕСТЬ

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}