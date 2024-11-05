using FP.Domain.Base;
using FP.Domain.Enums;

namespace FP.Domain
{
    public class Operation : BaseEntity
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public OperationType Type { get; set; }
        public Category Category { get; set; }
    }
}
