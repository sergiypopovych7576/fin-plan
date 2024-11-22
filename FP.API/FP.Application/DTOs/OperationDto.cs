using FluentValidation;
using FP.Domain;
using FP.Domain.Enums;

namespace FP.Application.DTOs
{
    public class OperationDto : BaseDto
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateOnly Date { get; set; }
        public OperationType Type { get; set; }
        public CategoryDto? Category { get; set; }
        public Guid CategoryId { get; set; }
        public bool Applied { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public Frequency? Frequency { get; set; }
        public int? Interval { get; set; }
        public Guid? ScheduledOperationId { get; set; }
        public ScheduledOperation? ScheduledOperation { get; set; }
		public Account? SourceAccount { get; set; }
		public Guid? SourceAccountId { get; set; }
		public Account? TargetAccount { get; set; }
		public Guid? TargetAccountId { get; set; }
	}

    public class OperationValidator : AbstractValidator<OperationDto>
    {
        public OperationValidator()
        {
            // TODO Check for types + account currency
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Amount).GreaterThan(0);
            RuleFor(c => c.CategoryId).NotEmpty();
            RuleFor(c => c.StartDate)
             .Null()
             .When(c => !c.Frequency.HasValue);
            RuleFor(c => c.EndDate)
               .Null()
               .When(c => !c.Frequency.HasValue);
            RuleFor(c => c.Interval)
             .Null()
             .When(c => !c.Frequency.HasValue);
            RuleFor(c => c.Interval)
              .GreaterThan(0)
              .When(c => c.Frequency.HasValue);
            RuleFor(c => c.StartDate)
             .NotEmpty()
             .When(c => c.Frequency.HasValue);
        }
    }
}
