using FP.Application.DTOs;
using FP.Application.Interfaces;
using FP.Domain;
using FP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FP.Application.Services
{
    public interface IScheduledOperationsService : IBaseService
    {
        Task Create(ScheduledOperation operation);
        Task<List<Operation>> GetPlannedScheduledOperationsUpToMonth(DateOnly targeDate);
        Task<List<Operation>> GetPlannedScheduledOperationsForMonth(DateOnly targeDate);
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

        public async Task<List<Operation>> GetPlannedScheduledOperationsForMonth(DateOnly targeDate)
        {
            var allOperations = await GetPlannedScheduledOperationsUpToMonth(targeDate);
            var monthOperations = allOperations.Where(c => c.Date.Month == targeDate.Month && c.Date.Year == targeDate.Year)
                .ToList();
            //monthOperations.ForEach(c =>  c.Date = new DateOnly(targeDate.Year, targeDate.Month, schedule.StartDate.Day));
            // TODO Match with schedule
            return monthOperations;
        }

        public async Task<List<Operation>> GetPlannedScheduledOperationsUpToMonth(DateOnly targeDate)
        {
            var scheduledOperations = await _repository.GetAll()
                .Where(s => s.StartDate <= targeDate
                    && (s.EndDate == null || s.EndDate >= targeDate)
                    && s.Interval != 0)
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
    }
}
