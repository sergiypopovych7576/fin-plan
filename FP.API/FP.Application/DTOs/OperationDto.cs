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
    }
}
