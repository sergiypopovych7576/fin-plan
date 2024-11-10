using FP.Domain.Base;
using FP.Domain.Enums;

namespace FP.Domain
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public OperationType Type { get; set; }
    }
}
