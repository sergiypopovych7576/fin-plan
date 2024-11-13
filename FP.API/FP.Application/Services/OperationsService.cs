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
        Task<List<OperationDto>> GetMonthlyOperations(int year, int month, CancellationToken cancellationToken);
        Task Create(OperationDto operation);
        Task Delete(Guid id);
        Task Sync();
    }

    public class OperationsService : IOperationsService
    {
        private readonly IRepository<Account> _accRepo;
        private readonly IRepository<Operation> _repo;
        private readonly IAccountService _accService;
        private readonly IMapper _mapper;

        public OperationsService(IRepository<Operation> repo, IAccountService accService, IRepository<Account> accRepo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
            _accService = accService;
            _accRepo = accRepo;
        }

        public async Task Delete(Guid id)
        {
            var operation = await _repo.GetByIdAsync(id);
            _repo.Remove(operation);
            if (!operation.Applied)
            {
                await _repo.SaveChangesAsync();
                return;
            }
            var defaultAccount = await _accService.GetDefault();
            var amount = operation.Amount;
            if (operation.Type == OperationType.Income)
            {
                amount = -amount;
            }
            defaultAccount.Balance += amount;
            await _accService.Update(defaultAccount);
        }

        public async Task Create(OperationDto operation)
        {
            var defaultAccount = await _accService.GetDefault();
            var mappedOp = _mapper.Map<Operation>(operation);
            if (operation.Date > DateTime.UtcNow)
            {
                await _repo.AddAsync(mappedOp);
                await _repo.SaveChangesAsync();
                return;
            }
            var amount = operation.Amount;
            if (operation.Type == OperationType.Expense)
            {
                amount = -amount;
            }
            defaultAccount.Balance += amount;
            mappedOp.Applied = true;
            await _repo.AddAsync(mappedOp);
            await _accService.Update(defaultAccount);
        }

        public async Task Sync()
        {
            var notAppliedOperations = await _repo.GetAll().Where(o => !o.Applied 
            //&& o.Date <= DateTime.UtcNow
            ).ToListAsync();
            var defaultAccount = await _accService.GetDefault();
            foreach (var operation in notAppliedOperations) {
                var amount = operation.Amount;
                if (operation.Type == OperationType.Expense)
                {
                    amount = -amount;
                }
                defaultAccount.Balance += amount;
                operation.Applied = true;
            }
            _repo.Update(notAppliedOperations);
            await _accService.Update(defaultAccount);
        }

        public async Task<List<OperationDto>> GetMonthlyOperations(int year, int month, CancellationToken cancellationToken)
        {
            return await _mapper.ProjectTo<OperationDto>(_repo.GetAll().Include(c => c.Category)
              .Where(o => o.Date.Month == month && o.Date.Year == year).OrderBy(c => c.Date))
              .ToListAsync(cancellationToken);
        }
    }
}
