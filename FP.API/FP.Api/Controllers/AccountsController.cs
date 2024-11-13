using AutoMapper;
using FP.Application.DTOs;
using FP.Application.Interfaces;
using FP.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IRepository<Account> _repo;
        private readonly IMapper _mapper;

        public AccountsController(IRepository<Account> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public Task<List<AccountDto>> Get()
        {
            return _mapper.ProjectTo<AccountDto>(_repo.GetAll().AsNoTracking()).ToListAsync();
        }

        [HttpPost]
        public Task Post(AccountDto account)
        {
            _repo.AddAsync(_mapper.Map<Account>(account));
            return _repo.SaveChangesAsync();
        }

        [HttpPut]
        public Task Put(AccountDto account)
        {
            _repo.Update(_mapper.Map<Account>(account));
            return _repo.SaveChangesAsync();
        }

        [HttpDelete("{id}")]
        public Task Delete(Guid id)
        {
            _repo.Remove(new Account { Id = id });
            return _repo.SaveChangesAsync();
        }
    }
}
