using AutoMapper;
using FP.Application.DTOs;
using FP.Application.Interfaces;
using FP.Domain;
using FP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FP.Application.Services
{
    public interface IOperationsService : IBaseService
    {
        Task<List<OperationDto>> GetMonthlyOperations(DateOnly date, CancellationToken cancellationToken);
        Task<List<MonthSummaryDto>> GetSummaryByDateRange(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken);
        Task Update(OperationDto operation);
        Task Create(OperationDto operation);
        Task Delete(Guid id);
        Task Sync();
    }

    public class OperationsService : IOperationsService
    {
        private readonly IRepository<Operation> _repo;
        private readonly IScheduledOperationsService _scheduledOperationsService;
        private readonly IAccountService _accService;
        private readonly IMapper _mapper;

        public OperationsService(
            IRepository<Operation> repo,
            IAccountService accService,
            IScheduledOperationsService scheduledOperationsService,
            IMapper mapper)
        {
            _repo = repo;
            _accService = accService;
            _scheduledOperationsService = scheduledOperationsService;
            _mapper = mapper;
        }

        public async Task Delete(Guid id)
        {
            var operation = await _repo.GetAll().FirstOrDefaultAsync(c => c.Id == id);
            if (operation == null || operation.ScheduledOperationId != null)
            {
                var schedOpId = operation?.ScheduledOperationId ?? id;
                var scheduledOperation = await _scheduledOperationsService.GetById(id);
                var associatedOperations = await _repo.GetAll().Where(c => c.ScheduledOperationId == id).ToListAsync();

                await _accService.RemoveOperations(associatedOperations);

                await _scheduledOperationsService.Delete(scheduledOperation);
                _repo.RemoveRange(associatedOperations);
                await _repo.SaveChangesAsync();

                return;
            }
            _repo.Remove(operation);
            if (!operation.Applied)
            {
                await _repo.SaveChangesAsync();
                return;
            }
            await _accService.RemoveOperation(operation);
        }

        public async Task Create(OperationDto operationDto)
        {
            if (operationDto.Frequency != null)
            {
                await _scheduledOperationsService.Create(_mapper.Map<ScheduledOperation>(operationDto));
                await Sync();
                return;
            }

            var operation = _mapper.Map<Operation>(operationDto);
            await _repo.AddAsync(operation);
            await _repo.SaveChangesAsync();
            await Sync();
        }

        public async Task Update(OperationDto operationDto)
        {
            var originalOperation = await _repo.GetAll().AsNoTracking().FirstOrDefaultAsync(c => c.Id == operationDto.Id);
            if (originalOperation == null || originalOperation.ScheduledOperationId != null)
            {
                return;
            }
            var newOperation = _mapper.Map<Operation>(operationDto);
            _repo.Update(newOperation);
            await _repo.SaveChangesAsync();
            await _accService.RemoveOperation(originalOperation);
            await _accService.ApplyOperation(newOperation);
        }

        public async Task Sync()
        {
            var now = DateOnly.FromDateTime(DateTime.UtcNow);

            // Get not applied operations and first not applied date
            var notAppliedOperations = await GetNotAppliedOperationsAsync(now);

            // Get scheduled operations for all accounts
            var scheduledOperations = await GetScheduledOperationsForAccountsUpTo(now);

            // Filter out already applied scheduled 
            var appliedOperations = GetAppliedScheduledOperations(now);
            var filteredScheduledOperations = FilterOutAppliedScheduledOperations(scheduledOperations, appliedOperations);

            // Combine and apply operations
            var operationsToApply = notAppliedOperations.Concat(filteredScheduledOperations).ToList();
            operationsToApply.ForEach(o => o.Applied = true);

            await _repo.AddRangeAsync(filteredScheduledOperations);
            _repo.Update(notAppliedOperations);
            await _accService.ApplyOperations(operationsToApply);
        }

        public async Task<List<OperationDto>> GetMonthlyOperations(DateOnly date, CancellationToken cancellationToken)
        {
            var operations = await GetOperationsForMonthAsync(date, cancellationToken);

            // Get scheduled operations for all accounts
            var scheduledOperations = await GetScheduledOperationsForAccountsByMonth(date);

            // Map `OperationDto` to `Operation` for filtering
            var mappedOperations = _mapper.Map<List<Operation>>(operations);

            // Filter out already applied scheduled operations
            var filteredScheduledOperations = FilterOutAppliedScheduledOperations(scheduledOperations, mappedOperations);

            operations.AddRange(_mapper.Map<List<OperationDto>>(filteredScheduledOperations));
            return operations.OrderBy(o => o.Date).ThenBy(o => o.Type).ToList();
        }


        public async Task<List<MonthSummaryDto>> GetSummaryByDateRange(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("startDate must be earlier than or equal to endDate.");
            }

            var operations = await GetOperationsByDateRangeAsync(startDate, endDate, cancellationToken);
            var scheduledOperations = await _scheduledOperationsService.GetPlannedScheduledOperationsByDateRange(startDate, endDate);

            var filteredScheduledOperations = FilterOutAppliedScheduledOperations(scheduledOperations, operations);
            var allOperations = operations.Concat(filteredScheduledOperations);

            return GenerateMonthlySummaries(allOperations);
        }

        // Helper Methods

        private async Task<List<Operation>> GetNotAppliedOperationsAsync(DateOnly now)
        {
            return await _repo.GetAll()
                .Where(o => !o.Applied && o.Date <= now)
                .ToListAsync();
        }

        private IEnumerable<Operation> GetAppliedScheduledOperations(DateOnly now)
        {
            var query = _repo.GetAll()
                .Where(o => o.Applied && o.Date <= now && o.ScheduledOperationId != null);

            return query.ToList();
        }

        private async Task<List<Operation>> GetScheduledOperationsForAccountsUpTo(DateOnly now)
        {
            var accounts = await _accService.GetAccounts();
            var scheduledOperations = new List<Operation>();

            foreach (var account in accounts)
            {
                scheduledOperations.AddRange(await _scheduledOperationsService.GetPlannedScheduledOperationsUpToMonth(account.Id, now));
            }

            return scheduledOperations;
        }

        private async Task<List<Operation>> GetScheduledOperationsForAccountsByMonth(DateOnly date)
        {
            var accounts = await _accService.GetAccounts();
            var scheduledOperations = new List<Operation>();

            foreach (var account in accounts)
            {
                scheduledOperations.AddRange(await _scheduledOperationsService.GetPlannedScheduledOperationsForMonth(account.Id, date));
            }

            return scheduledOperations;
        }

        private IEnumerable<Operation> FilterOutAppliedScheduledOperations(IEnumerable<Operation> scheduledOperations, IEnumerable<Operation> appliedOperations)
        {
            return scheduledOperations.Where(scheduled => !appliedOperations.Any(applied =>
                applied.ScheduledOperationId == scheduled.ScheduledOperationId &&
                applied.Date.Month == scheduled.Date.Month &&
                applied.Date.Year == scheduled.Date.Year &&
                applied.Amount == scheduled.Amount));
        }


        private async Task<List<OperationDto>> GetOperationsForMonthAsync(DateOnly date, CancellationToken cancellationToken)
        {
            return await _mapper.ProjectTo<OperationDto>(
                _repo.GetAll()
                    .AsNoTracking()
                    .Include(o => o.Category)
                    .Where(o => o.Date.Year == date.Year && o.Date.Month == date.Month)
                    .OrderBy(o => o.Date))
                .ToListAsync(cancellationToken);
        }

        private async Task<List<Operation>> GetOperationsByDateRangeAsync(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
        {
            return await _repo.GetAll()
                .AsNoTracking()
                .Include(o => o.Category)
                .Where(o => o.Date >= startDate && o.Date <= endDate && o.Type != OperationType.Transfer)
                .ToListAsync(cancellationToken);
        }

        private List<MonthSummaryDto> GenerateMonthlySummaries(IEnumerable<Operation> allOperations)
        {
            return allOperations
                .GroupBy(op => new { op.Date.Year, op.Date.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(group => new MonthSummaryDto
                {
                    Year = group.Key.Year,
                    Month = group.Key.Month,
                    TotalIncomes = group.Where(op => op.Type == OperationType.Income).Sum(op => op.Amount),
                    TotalExpenses = group.Where(op => op.Type == OperationType.Expense).Sum(op => op.Amount),
                    MonthBalance = group.Where(op => op.Type == OperationType.Income).Sum(op => op.Amount) -
                                   group.Where(op => op.Type == OperationType.Expense).Sum(op => op.Amount),
                    Categories = group.GroupBy(op => op.Category.Name)
                        .Select(categoryGroup => new CategorySummaryDto
                        {
                            Name = categoryGroup.Key,
                            Amount = categoryGroup.Sum(op => op.Amount),
                            Type = categoryGroup.First().Type,
                            Color = categoryGroup.First().Category.Color
                        })
                        .ToList()
                })
                .ToList();
        }
    }

}
