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

        public AccountService(IRepository<Account> accRepo, IRepository<Operation> opRepo,
            IDateService dateService, IScheduledOperationsService scheduledOperationsService, IMapper mapper)
        {
            _accRepo = accRepo;
            _scheduledOperationsService = scheduledOperationsService;
            _opRepo = opRepo;
            _dateService = dateService;
            _mapper = mapper;
        }

        public Task<List<Account>> GetAccounts() {
			return _accRepo.GetAll().AsNoTracking().ToListAsync();
		}


		public async Task<AccountMonthBalanceSummaryDto> GetBalance(Guid accountId, DateOnly targetDate)
        {
            var today = DateOnly.FromDateTime(_dateService.GetUtcDate());
            var account = await _accRepo.GetByIdAsync(accountId);

			var currentMonthOperations = _opRepo.GetAll().AsNoTracking()
                .Where(c => c.Type != OperationType.Transfer && (c.SourceAccountId == accountId || c.TargetAccountId == accountId) && c.Date.Year == targetDate.Year && c.Date.Month == targetDate.Month).ToList();

			var scheduledOperations = await _scheduledOperationsService.GetPlannedScheduledOperationsUpToMonth(accountId, targetDate);
            var currentMonthScheduledOperations = scheduledOperations.Where(c => c.Date.Year == targetDate.Year && c.Date.Month == targetDate.Month);
            var currentMonthNotAppliedScheduledOperations = currentMonthScheduledOperations.Where(c => !currentMonthOperations.Any(f => c.ScheduledOperationId == f.ScheduledOperationId && c.Date == f.Date));

            var previousScheduledOperations = scheduledOperations.Except(currentMonthScheduledOperations);
            var previousAppliedScheduledOperations = _opRepo.GetAll().AsNoTracking().Where(c => c.Date < targetDate && c.ScheduledOperationId != null && c.Applied).ToList();
            var previouslyNotAppliedScheduledOperations = previousScheduledOperations.Where(c => !previousAppliedScheduledOperations.Any(f => c.ScheduledOperationId == f.ScheduledOperationId && c.Date == f.Date));

            var currentMonthOperationIds = currentMonthOperations.Select(v => v.Id);
            var previousNotAppliedOperations = _opRepo.GetAll().AsNoTracking().Where(c => c.Date < targetDate && !c.Applied && !currentMonthOperationIds.Contains(c.Id) && (c.SourceAccountId == accountId || c.TargetAccountId == accountId)).ToList();
            var startingBalance = account.Balance;
            foreach (var previousNotAppliedOperation in previousNotAppliedOperations.Concat(previouslyNotAppliedScheduledOperations))
            {
                startingBalance = OperationCalcService.ApplyOperation(startingBalance, previousNotAppliedOperation);
            }
            var endMonthBalance = startingBalance;
            var currentMonthNotAppliedOperations = currentMonthOperations.Where(c => !c.Applied);
			foreach (var operation in currentMonthNotAppliedOperations.Concat(currentMonthNotAppliedScheduledOperations))
            {
                endMonthBalance = OperationCalcService.ApplyOperation(endMonthBalance, operation);
            }

            var currentMonthExpenses = currentMonthOperations.Where(c => c.Type == OperationType.Expense).Concat(currentMonthNotAppliedScheduledOperations.Where(s => s.Type == OperationType.Expense)).Sum(c => c.Amount);
			var currentMonthIncomes = currentMonthOperations.Where(c => c.Type == OperationType.Income).Concat(currentMonthNotAppliedScheduledOperations.Where(s => s.Type == OperationType.Income)).Sum(c => c.Amount);

			return new AccountMonthBalanceSummaryDto
            {
                StartMonthBalance = startingBalance,
                Difference = currentMonthIncomes - currentMonthExpenses,
				EndMonthBalance = endMonthBalance,
            };
        }

        public Task<Account> GetDefault()
        {
            return _accRepo.GetAll().AsNoTracking().FirstAsync(c => c.IsDefault);
        }

        public async Task ApplyOperations(List<Operation> operations)
        {
            var accounts = await _accRepo.GetAll().ToListAsync();
            foreach (var operation in operations)
            {
                if(operation.Type == OperationType.Income)
				{
					var targetAccount = accounts.First(c => c.Id == operation.TargetAccountId);
					targetAccount.Balance = OperationCalcService.ApplyOperation(targetAccount.Balance, operation);
				}
				if (operation.Type == OperationType.Expense) {
					var sourceAccount = accounts.First(c => c.Id == operation.SourceAccountId);
					sourceAccount.Balance = OperationCalcService.ApplyOperation(sourceAccount.Balance, operation);
				}
				if (operation.Type == OperationType.Transfer) {
                    var sourceAccount = accounts.First(c => c.Id == operation.SourceAccountId);
					var targetAccount = accounts.First(c => c.Id == operation.TargetAccountId);
					operation.Type = OperationType.Expense;
					sourceAccount.Balance = OperationCalcService.ApplyOperation(sourceAccount.Balance, operation);
					operation.Type = OperationType.Income;
					targetAccount.Balance = OperationCalcService.ApplyOperation(targetAccount.Balance, operation);
					operation.Type = OperationType.Transfer;
				}
			}
            _accRepo.Update(accounts);
            await _accRepo.SaveChangesAsync();
        }

        public async Task RemoveOperations(List<Operation> operations)
        {
			var accounts = await _accRepo.GetAll().ToListAsync();
			foreach (var operation in operations)
            {
				if (operation.Type == OperationType.Income) {
					var targetAccount = accounts.First(c => c.Id == operation.TargetAccountId);
					targetAccount.Balance = OperationCalcService.RemoveOperation(targetAccount.Balance, operation);
				}
				if (operation.Type == OperationType.Expense) {
					var sourceAccount = accounts.First(c => c.Id == operation.SourceAccountId);
					sourceAccount.Balance = OperationCalcService.RemoveOperation(sourceAccount.Balance, operation);
				}
				if (operation.Type == OperationType.Transfer) {
					var sourceAccount = accounts.First(c => c.Id == operation.SourceAccountId);
					var targetAccount = accounts.First(c => c.Id == operation.TargetAccountId);
					operation.Type = OperationType.Income;
					sourceAccount.Balance = OperationCalcService.RemoveOperation(sourceAccount.Balance, operation);
					operation.Type = OperationType.Expense;
					targetAccount.Balance = OperationCalcService.RemoveOperation(targetAccount.Balance, operation);
					operation.Type = OperationType.Transfer;
				}
            }
            _accRepo.Update(accounts);
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
