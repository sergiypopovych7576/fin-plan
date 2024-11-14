namespace FP.Application.DTOs
{
    public class AccountDto : BaseDto
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public bool IsDefault { get; set; }
        public string Currency { get; set; }
    }
}
