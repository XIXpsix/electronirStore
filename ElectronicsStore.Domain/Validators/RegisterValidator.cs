using ElectronicsStore.Domain.ViewModels;
using FluentValidation;

namespace ElectronicsStore.Domain.Validators
{
    public class RegisterValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(3, 50).WithMessage("Имя должно быть от 3 до 50 символов.");
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Введите корректный Email.");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Пароль должен быть не менее 6 символов.");
            RuleFor(x => x.PasswordConfirm)
                .Equal(x => x.Password).WithMessage("Пароли не совпадают.");
        }
    }
}