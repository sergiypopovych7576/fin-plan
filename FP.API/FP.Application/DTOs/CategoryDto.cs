using FluentValidation;
using FP.Domain.Enums;

namespace FP.Application.DTOs
{
    public class CategoryDto : BaseDto
    {
        public required string Name { get; set; }
        public required string Color { get; set; }
        public required string IconName { get; set; }
        public OperationType Type { get; set; }
    }

    public class CategoryValidator : AbstractValidator<CategoryDto>
    {
        public CategoryValidator()
        {
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Color).NotEmpty();
            RuleFor(c => c.IconName).NotEmpty();
        }
    }
}
