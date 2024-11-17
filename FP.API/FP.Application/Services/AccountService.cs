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
        Task<AccountMonthBalanceSummaryDto> GetBalanceSummary(DateOnly targetDate);
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

        public AccountService(IRepository<Account> accRepo, IRepository<Operation> opRepo,
            IDateService dateService, IScheduledOperationsService scheduledOperationsService, IMapper mapper)
        {
            _accRepo = accRepo;
            _scheduledOperationsService = scheduledOperationsService;
            _opRepo = opRepo;
            _dateService = dateService;
            _mapper = mapper;
        }

        public async Task<AccountMonthBalanceSummaryDto> GetBalanceSummary(DateOnly targetDate)
        {
            var today = DateOnly.FromDateTime(_dateService.GetUtcDate());
            var defaultAccount = await GetDefault();

            var currentMonthOperations = _opRepo.GetAll().AsNoTracking().Where(c => c.Date.Year == targetDate.Year && c.Date.Month == targetDate.Month).ToList();
            var monthExpenseOperations = currentMonthOperations.Where(c => c.Type == OperationType.Expense);
            var monthIncomeOperations = currentMonthOperations.Where(c => c.Type == OperationType.Income);

            var scheduledOperations = await _scheduledOperationsService.GetPlannedScheduledOperationsUpToMonth(targetDate);
            var currentMonthScheduledOperations = scheduledOperations.Where(c => c.Date.Year == targetDate.Year && c.Date.Month == targetDate.Month);
            var currentMonthNotAppliedScheduledOperations = currentMonthScheduledOperations.Where(c => !currentMonthOperations.Any(f => c.ScheduledOperationId == f.ScheduledOperationId && c.Date == f.Date));

            var previousScheduledOperations = scheduledOperations.Except(currentMonthScheduledOperations);
            var previousAppliedScheduledOperations = _opRepo.GetAll().AsNoTracking().Where(c => c.Date < targetDate && c.ScheduledOperationId != null && c.Applied).ToList();
            var previouslyNotAppliedScheduledOperations = previousScheduledOperations.Where(c => !previousAppliedScheduledOperations.Any(f => c.ScheduledOperationId == f.ScheduledOperationId && c.Date == f.Date));

            var monthExpenseSum = monthExpenseOperations.Sum(c => c.Amount) + currentMonthNotAppliedScheduledOperations.Where(c => c.Type == OperationType.Expense).Sum(c => c.Amount);
            var monthIncomeSum = monthIncomeOperations.Sum(c => c.Amount) + currentMonthNotAppliedScheduledOperations.Where(c => c.Type == OperationType.Income).Sum(c => c.Amount); ;

            var currentMonthOperationIds = currentMonthOperations.Select(v => v.Id);
            var previousNotAppliedOperations = _opRepo.GetAll().AsNoTracking().Where(c => c.Date < targetDate && !c.Applied && !currentMonthOperationIds.Contains(c.Id)).ToList();
            var startingBalance = defaultAccount.Balance;
            foreach (var previousNotAppliedOperation in previousNotAppliedOperations.Concat(previouslyNotAppliedScheduledOperations))
            {
                startingBalance = OperationCalcService.ApplyOperation(startingBalance, previousNotAppliedOperation);
            }
            var endMonthBalance = startingBalance;
            foreach (var operation in currentMonthOperations.Where(c => !c.Applied).Concat(currentMonthNotAppliedScheduledOperations))
            {
                endMonthBalance = OperationCalcService.ApplyOperation(endMonthBalance, operation);
            }

            return new AccountMonthBalanceSummaryDto
            {
                Balance = startingBalance,
                Expenses = monthExpenseSum,
                Incomes = monthIncomeSum,
                MonthBalance = monthIncomeSum - monthExpenseSum,
                EndMonthBalance = endMonthBalance,
            };
        }

        public Task<Account> GetDefault()
        {
            return _accRepo.GetAll().AsNoTracking().FirstAsync(c => c.IsDefault);
        }

        public async Task ApplyOperations(List<Operation> operations)
        {
            var defaultAccount = await GetDefault();
            foreach (var operation in operations)
            {
                defaultAccount.Balance = OperationCalcService.ApplyOperation(defaultAccount.Balance, operation);
            }
            _accRepo.Update(defaultAccount);
            await _accRepo.SaveChangesAsync();
        }

        public async Task RemoveOperations(List<Operation> operations)
        {
            var defaultAccount = await GetDefault();
            foreach (var operation in operations)
            {
                defaultAccount.Balance = OperationCalcService.RemoveOperation(defaultAccount.Balance, operation);
            }
            _accRepo.Update(defaultAccount);
            await _accRepo.SaveChangesAsync();
        }

        public Task ApplyOperation(Operation operation)
        {
            return ApplyOperations(new List<Operation> { operation });
        }

        public Task RemoveOperation(Operation operation)
        {
            return RemoveOperations(new List<Operation> { operation });
        }
    }
}
