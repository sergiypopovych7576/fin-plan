using FluentValidation;
using FP.Application.DTOs;

namespace FP.Application.Validations
{
    internal class CategoryValidator : AbstractValidator<CategoryDto>
    {
        public CategoryValidator()
        {
            RuleFor(c => c.Id).NotEmpty();
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Color).NotEmpty();
        }
    }
}
