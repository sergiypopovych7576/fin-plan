using AutoMapper;
using FP.Application.DTOs;
using FP.Application.Interfaces;
using FP.Domain;
using Microsoft.EntityFrameworkCore;

namespace FP.Application.Services
{
    public interface IAccountService : IBaseService
    {
        Task<List<AccountDto>> Get();
        Task<AccountDto> GetDefault();
        Task Update(AccountDto account);
    }


    public class AccountService : IAccountService
    {
        private readonly IRepository<Account> _accRepo;
        private readonly IMapper _mapper;

        public AccountService(IRepository<Account> accRepo, IMapper mapper)
        {
            _accRepo = accRepo;
            _mapper = mapper;
        }

        public Task<List<AccountDto>> Get()
        {
            return _mapper.ProjectTo<AccountDto>(_accRepo.GetAll()).ToListAsync();
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
    }
}
