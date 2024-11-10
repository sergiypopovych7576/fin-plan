using FP.Domain.Enums;

namespace FP.Application.DTOs
{
    public class CategoryDto : BaseDto
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public OperationType Type { get; set; }
    }
}
