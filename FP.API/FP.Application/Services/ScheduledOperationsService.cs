using FP.Application.Interfaces;
using FP.Domain;
using FP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FP.Application.Services
{
    public interface IScheduledOperationsService : IBaseService
    {
        ValueTask<ScheduledOperation> GetById(Guid id);
        Task Create(ScheduledOperation operation);
        Task Delete(ScheduledOperation operation);
        Task<List<Operation>> GetPlannedScheduledOperationsUpToMonth(Guid accountId, DateOnly targetDate);
        Task<List<Operation>> GetPlannedScheduledOperationsForMonth(Guid accountId, DateOnly targetDate);
        Task<List<Operation>> GetPlannedScheduledOperationsByDateRange(DateOnly startDate, DateOnly endDate);
    }

    public class ScheduledOperationsService : IScheduledOperationsService
    {
        private readonly IRepository<ScheduledOperation> _repository;

        public ScheduledOperationsService(IRepository<ScheduledOperation> repository)
        {
            _repository = repository;
        }

        public async Task Create(ScheduledOperation operation)
        {
            await _repository.AddAsync(operation);
            await _repository.SaveChangesAsync();
        }

        public async Task<List<Operation>> GetPlannedScheduledOperationsForMonth(Guid accountId, DateOnly targetDate)
        {
            var startOfMonth = new DateOnly(targetDate.Year, targetDate.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            return await GetPlannedScheduledOperationsByDateRangeAndAccount(accountId, startOfMonth, endOfMonth);
        }

        public async Task<List<Operation>> GetPlannedScheduledOperationsUpToMonth(Guid accountId, DateOnly targetDate)
        {
            var startOfYear = new DateOnly(targetDate.Year - 1, 1, 1);
            return await GetPlannedScheduledOperationsByDateRangeAndAccount(accountId, startOfYear, targetDate);
        }

        private IQueryable<ScheduledOperation> GetQuery(DateOnly startDate, DateOnly endDate, Guid? accountId = null)
        {
            var query = _repository.GetAll()
                .Where(s => s.StartDate <= endDate
                    && (s.EndDate == null || s.EndDate >= startDate)
                    && s.Interval > 0);
            if (accountId.HasValue)
            {
                query = query.Where(s => s.SourceAccountId == accountId || s.TargetAccountId == accountId);
            }
            return query.Include(s => s.Category)
                .Include(s => s.SourceAccount)
                .Include(s => s.TargetAccount);
        }

        public async Task<List<Operation>> GetPlannedScheduledOperationsByDateRange(DateOnly startDate, DateOnly endDate)
        {
            var scheduledOperations = await GetQuery(startDate, endDate)
                .ToListAsync();
            return GenerateOperationsWithinRange(scheduledOperations, startDate, endDate);
        }

        private async Task<List<Operation>> GetPlannedScheduledOperationsByDateRangeAndAccount(Guid accountId, DateOnly startDate, DateOnly endDate)
        {
            var scheduledOperations = await GetQuery(startDate, endDate, accountId)
                .ToListAsync();

            return GenerateOperationsWithinRange(scheduledOperations, startDate, endDate);
        }

        private List<Operation> GenerateOperationsWithinRange(List<ScheduledOperation> scheduledOperations, DateOnly startDate, DateOnly endDate)
        {
            var result = new List<Operation>();

            foreach (var schedule in scheduledOperations)
            {
                var current = GetNextOccurrence(schedule.StartDate, startDate, schedule.Frequency, schedule.Interval);

                while (current <= endDate && (schedule.EndDate == null || current <= schedule.EndDate))
                {
                    result.Add(new Operation
                    {
                        Id = Guid.NewGuid(),
                        Name = schedule.Name,
                        Amount = schedule.Amount,
                        Date = current,
                        Type = schedule.Type,
                        Category = schedule.Category,
                        CategoryId = schedule.CategoryId,
                        ScheduledOperationId = schedule.Id,
                        TargetAccount = schedule.TargetAccount,
                        SourceAccount = schedule.SourceAccount,
                        SourceAccountId = schedule.SourceAccountId,
                        TargetAccountId = schedule.TargetAccountId,
                        Applied = false
                    });

                    current = schedule.Frequency switch
                    {
                        Frequency.Daily => current.AddDays(schedule.Interval),
                        Frequency.Weekly => current.AddDays(7 * schedule.Interval),
                        Frequency.Monthly => current.AddMonths(schedule.Interval),
                        Frequency.Yearly => current.AddYears(schedule.Interval),
                        _ => throw new InvalidOperationException("Invalid frequency")
                    };
                }
            }

            return result;
        }

        private DateOnly GetNextOccurrence(DateOnly startDate, DateOnly fromDate, Frequency frequency, int interval)
        {
            var current = startDate;

            while (current < fromDate)
            {
                current = frequency switch
                {
                    Frequency.Daily => current.AddDays(interval),
                    Frequency.Weekly => current.AddDays(7 * interval),
                    Frequency.Monthly => current.AddMonths(interval),
                    Frequency.Yearly => current.AddYears(interval),
                    _ => throw new InvalidOperationException("Invalid frequency")
                };
            }

            return current;
        }

        public ValueTask<ScheduledOperation> GetById(Guid id)
        {
            return _repository.GetByIdAsync(id);
        }

        public async Task Delete(ScheduledOperation operation)
        {
            _repository.Remove(operation);
            await _repository.SaveChangesAsync();
        }
    }
}
