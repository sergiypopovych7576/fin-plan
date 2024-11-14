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
        Task<AccountMonthBalanceSummaryDto> GetBalanceSummary(DateTime targetDate);
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
        private readonly IMapper _mapper;

        public AccountService(IRepository<Account> accRepo, IRepository<Operation> opRepo, IMapper mapper)
        {
            _accRepo = accRepo;
            _opRepo = opRepo;
            _mapper = mapper;
        }

        public async Task<AccountMonthBalanceSummaryDto> GetBalanceSummary(DateTime targetDate)
        {
            var today = DateTime.Now;
            var defaultAccount = await GetDefault();
            var previousNotAppliedOperations = new List<Operation>();
            if (targetDate.Month == today.Month && targetDate.Year == today.Year)
            {
                previousNotAppliedOperations = await _opRepo.GetAll().AsNoTracking()
            .Where(o => o.Date.Year == targetDate.Year && o.Date.Month == targetDate.Month).ToListAsync();
            } else
            {
                previousNotAppliedOperations = await _opRepo.GetAll().AsNoTracking()
              .Where(o => !o.Applied && o.Date <= targetDate).ToListAsync();
            }
            var currentMonthOperations = previousNotAppliedOperations.Where(o => o.Date.Month == targetDate.Month && o.Date.Year == targetDate.Year);
            var previousMonthOperations = previousNotAppliedOperations.Except(currentMonthOperations);
            var startingBalance = defaultAccount.Balance;
            foreach (var operation in previousMonthOperations)
            {
                var amount = operation.Amount;
                if (operation.Type == OperationType.Expense)
                {
                    amount = -amount;
                }
                startingBalance += amount;
            }
            var endBalance = startingBalance;
            foreach (var operation in currentMonthOperations.Where(c => !c.Applied))
            {
                var amount = operation.Amount;
                if (operation.Type == OperationType.Expense)
                {
                    amount = -amount;
                }
                endBalance += amount;
            }
            var monthExpenses = currentMonthOperations.Where(c => c.Type == OperationType.Expense).Sum(c => c.Amount);
            var monthIncomes = currentMonthOperations.Where(c => c.Type == OperationType.Income).Sum(c => c.Amount);
            return new AccountMonthBalanceSummaryDto
            {
                Balance = startingBalance,
                Expenses = monthExpenses,
                Incomes = monthIncomes,
                MonthBalance = monthIncomes - monthExpenses,
                EndMonthBalance = endBalance,
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
