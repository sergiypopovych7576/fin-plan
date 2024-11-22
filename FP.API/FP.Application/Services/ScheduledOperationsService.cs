using FP.Application.Interfaces;
using FP.Domain;
using FP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FP.Application.Services
{
	public interface IScheduledOperationsService : IBaseService
    {
        Task Create(ScheduledOperation operation);
        Task<List<Operation>> GetPlannedScheduledOperationsUpToMonth(Guid accountId, DateOnly targeDate);
        Task<List<Operation>> GetPlannedScheduledOperationsForMonth(Guid accountId, DateOnly targeDate);
        Task<List<Operation>> GetPlannedScheduledOperationsByDateRange(DateOnly startDate, DateOnly targeDate);
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

        public async Task<List<Operation>> GetPlannedScheduledOperationsForMonth(Guid accountId, DateOnly targeDate)
        {
            var allOperations = await GetPlannedScheduledOperationsUpToMonth(accountId, targeDate);
            var monthOperations = allOperations.Where(c => c.Date.Month == targeDate.Month && c.Date.Year == targeDate.Year)
                .ToList();
            //monthOperations.ForEach(c =>  c.Date = new DateOnly(targeDate.Year, targeDate.Month, schedule.StartDate.Day));
            // TODO Match with schedule
            return monthOperations;
        }

        public async Task<List<Operation>> GetPlannedScheduledOperationsUpToMonth(Guid accountId, DateOnly targeDate)
        {
            var scheduledOperations = await _repository.GetAll()
                .Where(s => s.StartDate <= targeDate
                    && (s.EndDate == null || s.EndDate >= targeDate)
                    && s.Interval != 0
                    && (s.SourceAccountId == accountId
                    || s.TargetAccountId == accountId))
                .Include(s => s.Category)
                .ToListAsync();

            var result = new List<Operation>();
            foreach (var schedule in scheduledOperations)
            {
                var current = schedule.StartDate > targeDate
                    ? schedule.StartDate
                    : schedule.StartDate;

                while (current <= targeDate && (schedule.EndDate == null || current <= schedule.EndDate))
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
                    };
                }
            }

            return result;
        }

        public async Task<List<Operation>> GetPlannedScheduledOperationsByDateRange(DateOnly startDate, DateOnly endDate)
        {
            // Fetch scheduled operations that overlap with the date range
            var scheduledOperations = await _repository.GetAll()
                .Where(s => s.StartDate <= endDate && (s.EndDate == null || s.EndDate >= startDate) && s.Interval > 0)
                .Include(s => s.Category)
                .ToListAsync();

            var result = new List<Operation>();

            foreach (var schedule in scheduledOperations)
            {
                // Determine the first applicable date within the range
                var current = schedule.StartDate > startDate ? schedule.StartDate : startDate;

                // Generate operations within the range
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
                        Applied = false
                    });

                    // Increment the current date based on the schedule frequency and interval
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

    }
}
