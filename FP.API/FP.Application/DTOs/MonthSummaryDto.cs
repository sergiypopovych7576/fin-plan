namespace FP.Application.DTOs
{
    public class MonthSummaryDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalIncomes { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal MonthBalance { get; set; }
    }
}
