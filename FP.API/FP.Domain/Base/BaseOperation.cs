using FP.Domain.Enums;

namespace FP.Domain.Base
{
    public class BaseOperation : BaseEntity
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public OperationType Type { get; set; }
        public Category Category { get; set; }
        public Guid CategoryId { get; set; }
    }
}
