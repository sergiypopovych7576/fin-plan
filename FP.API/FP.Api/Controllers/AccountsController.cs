using FP.Application.DTOs;
using FP.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _service;

        public AccountsController(IAccountService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("balance")]
        public Task<AccountMonthBalanceSummaryDto> Get([FromQuery] DateTime targetDate)
        {
            return _service.GetBalanceSummary(targetDate);
        }

        [HttpGet]
        public Task<List<AccountDto>> Get()
        {
            return _service.Get();
        }

        [HttpPost]
        public Task Post(AccountDto account)
        {
            return _service.Create(account);
        }

        [HttpPut]
        public Task Put(AccountDto account)
        {
            return _service.Update(account);
        }

        [HttpDelete("{id}")]
        public Task Delete(Guid id)
        {
            return _service.Delete(id);
        }
    }
}
