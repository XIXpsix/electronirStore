using ElectronicsStore.Domain.ViewModels;
using FluentValidation;

namespace ElectronicsStore.Domain.Validators
{
    public class ProductValidator : AbstractValidator<ProductViewModel>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название обязательно")
                .Length(3, 100).WithMessage("Длина названия должна быть от 3 до 100 символов");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Описание обязательно");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Цена должна быть больше нуля");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Выберите категорию");
        }
    }
}