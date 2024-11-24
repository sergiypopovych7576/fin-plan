using AutoMapper;
using FP.Application.DTOs;
using FP.Application.Interfaces;
using FP.Domain;
using FP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FP.Application.Services
{
    public interface IAccountService : IBaseService
    {
        Task<AccountMonthBalanceSummaryDto> GetBalance(Guid accountId, DateOnly targetDate);
        Task<List<Account>> GetAccounts();
        Task ApplyOperation(Operation operation);
        Task RemoveOperation(Operation operation);
        Task ApplyOperations(List<Operation> operations);
        Task RemoveOperations(List<Operation> operations);
    }

    public class AccountService : IAccountService
    {
        private readonly IRepository<Operation> _opRepo;
        private readonly IRepository<Account> _accRepo;
        private readonly IDateService _dateService;
        private readonly IScheduledOperationsService _scheduledOperationsService;
        private readonly IMapper _mapper;

        public AccountService(
            IRepository<Account> accRepo,
            IRepository<Operation> opRepo,
            IDateService dateService,
            IScheduledOperationsService scheduledOperationsService,
            IMapper mapper)
        {
            _accRepo = accRepo;
            _opRepo = opRepo;
            _dateService = dateService;
            _scheduledOperationsService = scheduledOperationsService;
            _mapper = mapper;
        }

        public Task<List<Account>> GetAccounts() =>
            _accRepo.GetAll().AsNoTracking().ToListAsync();

        public async Task<AccountMonthBalanceSummaryDto> GetBalance(Guid accountId, DateOnly targetDate)
        {
            var today = DateOnly.FromDateTime(_dateService.GetUtcDate());
            var account = await _accRepo.GetByIdAsync(accountId);

            // Fetch operations
            var currentMonthOperations = await GetOperationsForMonth(accountId, targetDate);
            var scheduledOperations = await _scheduledOperationsService.GetPlannedScheduledOperationsUpToMonth(accountId, targetDate);
            var currentMonthScheduledOperations = FilterCurrentMonthOperations(scheduledOperations, targetDate);
            var currentMonthNotAppliedScheduledOperations = FilterNotAppliedOperations(currentMonthScheduledOperations, currentMonthOperations);

            // Handle previous operations
            var previousScheduledOperations = scheduledOperations.Except(currentMonthScheduledOperations);
            var previousAppliedScheduledOperations = await GetAppliedScheduledOperationsBefore(targetDate);
            var previouslyNotAppliedScheduledOperations = FilterNotAppliedOperations(previousScheduledOperations, previousAppliedScheduledOperations);

            var previousNotAppliedOperations = await GetPreviousNotAppliedOperations(accountId, targetDate, currentMonthOperations.Select(o => o.Id));

            // Calculate balances
            ApplyOperationsToAccount(account, previousNotAppliedOperations.Concat(previouslyNotAppliedScheduledOperations));
            var startingBalance = account.Balance;

            ApplyOperationsToAccount(account, currentMonthOperations.Where(o => !o.Applied).Concat(currentMonthNotAppliedScheduledOperations));
            var endMonthBalance = account.Balance;

            // Summarize incomes and expenses
            var (currentMonthIncomes, currentMonthExpenses) = CalculateIncomeAndExpenses(
                currentMonthOperations, currentMonthNotAppliedScheduledOperations);

            return new AccountMonthBalanceSummaryDto
            {
                StartMonthBalance = startingBalance,
                Difference = currentMonthIncomes - currentMonthExpenses,
                EndMonthBalance = endMonthBalance,
            };
        }

        public Task<Account> GetDefault() =>
            _accRepo.GetAll().AsNoTracking().FirstAsync(a => a.IsDefault);

        public async Task ApplyOperations(List<Operation> operations)
        {
            var accounts = await _accRepo.GetAll().ToListAsync();
            foreach (var operation in operations)
            {
                ApplyOperationToAccounts(accounts, operation);
            }
            _accRepo.Update(accounts);
            await _accRepo.SaveChangesAsync();
        }

        public async Task RemoveOperations(List<Operation> operations)
        {
            var accounts = await _accRepo.GetAll().ToListAsync();
            foreach (var operation in operations)
            {
                RemoveOperationFromAccounts(accounts, operation);
            }
            _accRepo.Update(accounts);
            await _accRepo.SaveChangesAsync();
        }

        public Task ApplyOperation(Operation operation) =>
            ApplyOperations(new List<Operation> { operation });

        public Task RemoveOperation(Operation operation) =>
            RemoveOperations(new List<Operation> { operation });

        // Helper Methods

        private async Task<List<Operation>> GetOperationsForMonth(Guid accountId, DateOnly targetDate) =>
            await _opRepo.GetAll()
                .AsNoTracking()
                .Where(o => (o.SourceAccountId == accountId || o.TargetAccountId == accountId) &&
                            o.Date.Year == targetDate.Year &&
                            o.Date.Month == targetDate.Month)
                .ToListAsync();

        private async Task<List<Operation>> GetAppliedScheduledOperationsBefore(DateOnly targetDate) =>
            await _opRepo.GetAll()
                .AsNoTracking()
                .Where(o => o.Date < targetDate && o.ScheduledOperationId != null && o.Applied)
                .ToListAsync();

        private async Task<List<Operation>> GetPreviousNotAppliedOperations(Guid accountId, DateOnly targetDate, IEnumerable<Guid> excludedOperationIds) =>
            await _opRepo.GetAll()
                .AsNoTracking()
                .Where(o => o.Date < targetDate &&
                            !o.Applied &&
                            !excludedOperationIds.Contains(o.Id) &&
                            (o.SourceAccountId == accountId || o.TargetAccountId == accountId))
                .ToListAsync();

        private IEnumerable<Operation> FilterCurrentMonthOperations(IEnumerable<Operation> operations, DateOnly targetDate) =>
            operations.Where(o => o.Date.Year == targetDate.Year && o.Date.Month == targetDate.Month);

        private IEnumerable<Operation> FilterNotAppliedOperations(IEnumerable<Operation> operations, IEnumerable<Operation> appliedOperations) =>
            operations.Where(o => !appliedOperations.Any(a => a.ScheduledOperationId == o.ScheduledOperationId && a.Date == o.Date));

        private void ApplyOperationsToAccount(Account account, IEnumerable<Operation> operations)
        {
            foreach (var operation in operations)
            {
                OperationCalcService.ApplyOperation(account, operation);
            }
        }

        private (decimal incomes, decimal expenses) CalculateIncomeAndExpenses(IEnumerable<Operation> operations, IEnumerable<Operation> notAppliedScheduledOperations)
        {
            var incomes = operations
                .Where(o => o.Type == OperationType.Income)
                .Concat(notAppliedScheduledOperations.Where(o => o.Type == OperationType.Income))
                .Sum(o => o.Amount);

            var expenses = operations
                .Where(o => o.Type == OperationType.Expense)
                .Concat(notAppliedScheduledOperations.Where(o => o.Type == OperationType.Expense))
                .Sum(o => o.Amount);

            return (incomes, expenses);
        }

        private void ApplyOperationToAccounts(List<Account> accounts, Operation operation)
        {
            switch (operation.Type)
            {
                case OperationType.Income:
                    var targetAccount = accounts.First(a => a.Id == operation.TargetAccountId);
                    OperationCalcService.ApplyOperation(targetAccount, operation);
                    break;
                case OperationType.Expense:
                    var sourceAccount = accounts.First(a => a.Id == operation.SourceAccountId);
                    OperationCalcService.ApplyOperation(sourceAccount, operation);
                    break;
                case OperationType.Transfer:
                    var source = accounts.First(a => a.Id == operation.SourceAccountId);
                    var target = accounts.First(a => a.Id == operation.TargetAccountId);
                    OperationCalcService.ApplyOperation(source, operation);
                    OperationCalcService.ApplyOperation(target, operation);
                    break;
            }
        }

        private void RemoveOperationFromAccounts(List<Account> accounts, Operation operation)
        {
            switch (operation.Type)
            {
                case OperationType.Income:
                    var targetAccount = accounts.First(a => a.Id == operation.TargetAccountId);
                    OperationCalcService.RemoveOperation(targetAccount, operation);
                    break;
                case OperationType.Expense:
                    var sourceAccount = accounts.First(a => a.Id == operation.SourceAccountId);
                    OperationCalcService.RemoveOperation(sourceAccount, operation);
                    break;
                case OperationType.Transfer:
                    var source = accounts.First(a => a.Id == operation.SourceAccountId);
                    var target = accounts.First(a => a.Id == operation.TargetAccountId);
                    OperationCalcService.RemoveOperation(source, operation);
                    OperationCalcService.RemoveOperation(target, operation);
                    break;
            }
        }
    }
}
