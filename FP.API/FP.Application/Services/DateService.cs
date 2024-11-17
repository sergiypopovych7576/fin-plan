using FP.Application.Interfaces;

namespace FP.Application.Services
{
    public interface IDateService : IBaseService
    {
        public DateTime GetUtcDate();
    }

    public class DateService : IDateService
    {
        public DateTime GetUtcDate()
        {
            return DateTime.UtcNow;
        }
    }
}