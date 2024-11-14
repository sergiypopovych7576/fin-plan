using FP.Domain.Base;

namespace FP.Domain
{
    public class Account : BaseEntity
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public bool IsDefault { get; set; }
        public string Currency { get; set; }
    }
}
