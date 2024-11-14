namespace FP.Application.DTOs
{
    public class AccountMonthBalanceSummaryDto
    {
        public decimal Balance { get; set; }
        public decimal Incomes { get; set; }
        public decimal Expenses { get; set; }
        public decimal MonthBalance { get; set; }
        public decimal EndMonthBalance { get; set; }
    }
}
