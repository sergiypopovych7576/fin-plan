using FP.Domain.Enums;

namespace FP.Application.DTOs
{
    public class OperationDto : BaseDto
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public OperationType Type { get; set; }
		public CategoryDto? Category { get; set; }
        public Guid CategoryId { get; set; }
    }
}
