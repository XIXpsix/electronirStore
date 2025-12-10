using ElectronicsStore.Domain.ViewModels;
using FluentValidation;

namespace ElectronicsStore.Domain.Validators
{
    public class LoginValidator : AbstractValidator<LoginViewModel>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Некорректный Email.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Введите пароль.");
        }
    }
}