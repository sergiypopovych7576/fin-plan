using FP.Domain.Base;

namespace FP.Domain
{
    public class Operation : BaseOperation
    {
        public bool Applied { get; set; }
        public DateOnly Date { get; set; }
        public Guid? ScheduledOperationId { get; set; }
        public ScheduledOperation ScheduledOperation { get; set; }
    }
}
