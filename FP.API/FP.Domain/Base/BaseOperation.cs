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
        public Account? SourceAccount { get; set; }
        public Guid? SourceAccountId { get; set; }
        public Account? TargetAccount { get; set; }
        public Guid? TargetAccountId { get; set; }
    }
}