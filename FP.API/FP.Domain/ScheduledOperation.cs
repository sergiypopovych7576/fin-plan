using FP.Domain.Base;
using FP.Domain.Enums;

namespace FP.Domain
{
    public class ScheduledOperation : BaseOperation
    {
        public DateOnly StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public Frequency Frequency { get; set; }
        public int Interval { get; set; }
    }
}
