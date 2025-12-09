using System.ComponentModel.DataAnnotations;

namespace ElectronicsStore.Models
{
    public class ConfirmEmailViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите код")]
        public string Code { get; set; } = string.Empty;
    }
}