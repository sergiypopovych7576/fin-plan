using FP.Domain.Enums;

namespace FP.Application.DTOs
{
    public class CategorySummaryDto
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Color { get; set; }
        public OperationType Type { get; set; }
    }
}
