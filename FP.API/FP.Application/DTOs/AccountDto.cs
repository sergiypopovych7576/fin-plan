using FluentValidation;

namespace FP.Application.DTOs
{
    public class AccountDto : BaseDto
    {
        public required string Name { get; set; }
        public decimal Balance { get; set; }
        public bool IsDefault { get; set; }
        public required string Currency { get; set; }
    }

    public class AccountValidator : AbstractValidator<AccountDto>
    {
        public AccountValidator()
        {
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Currency).NotEmpty();
        }
    }
}
