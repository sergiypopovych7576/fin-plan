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
        Task<List<AccountDto>> Get();
        Task<AccountDto> GetDefault();
        Task Update(AccountDto account);
        Task Create(AccountDto account);
        Task Delete(Guid id);
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

        public Task<List<AccountDto>> Get()
        {
            return _mapper.ProjectTo<AccountDto>(_accRepo.GetAll().AsNoTracking().OrderByDescending(c => c.IsDefault).ThenBy(c => c.Name)).ToListAsync();
        }

        public async Task<AccountDto> GetDefault()
        {
            var defaultAcc = await _accRepo.GetAll().AsNoTracking().FirstAsync(c => c.IsDefault);
            return _mapper.Map<AccountDto>(defaultAcc);
        }

        public async Task Update(AccountDto account)
        {
            _accRepo.Update(_mapper.Map<Account>(account));
            await _accRepo.SaveChangesAsync();
        }

        public Task Create(AccountDto account)
        {
            _accRepo.AddAsync(_mapper.Map<Account>(account));
            return _accRepo.SaveChangesAsync();
        }

        public Task Delete(Guid id)
        {
            _accRepo.Remove(new Account { Id = id });
            return _accRepo.SaveChangesAsync();
        }
    }
}
