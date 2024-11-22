using AutoMapper;
using FP.Application.DTOs;
using FP.Application.Interfaces;
using FP.Application.Services;
using FP.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : BaseCRUDController<Account, AccountDto, AccountValidator>
    {
        private readonly IAccountService _service;

        public AccountsController(IAccountService accountsService,
            IRepository<Account> repository, IMapper mapper, ICacheService cache) : base(repository, mapper, cache, "Accounts", c => !c.IsDefault, c => c.Name)
        {
            _service = accountsService;
        }

        [HttpGet]
        [Route("{accountId}/balance")]
        public Task<AccountMonthBalanceSummaryDto> Get(Guid accountId, [FromQuery] DateOnly targetDate)
        {
            return _service.GetBalance(accountId, targetDate);
        }
    }
}
