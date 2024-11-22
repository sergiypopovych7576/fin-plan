namespace FP.Application.DTOs
{
    public class AccountMonthBalanceSummaryDto
    {
        public decimal StartMonthBalance { get; set; }
        public decimal Difference { get; set; }
        public decimal EndMonthBalance { get; set; }
    }
}
