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
    public class OperationsController : BaseCRUDController<Operation, OperationDto, OperationValidator>
    {
        private readonly IOperationsService _service;

        public OperationsController(IOperationsService service,
            IRepository<Operation> repository, IMapper mapper, ICacheService cache)
            : base(repository, mapper, cache, "Operations", c => c.Name)
        {
            _service = service;
        }

        [HttpPost]
        [Route("sync")]
        public async Task Sync()
        {
            await _service.Sync();
            await _cache.Reset(_cacheKey);
            await _cache.Reset("Accounts");
        }

        [HttpGet]
        [Route("month/{date}")]
        public Task<List<OperationDto>> Get([FromRoute] DateOnly date, CancellationToken cancellationToken)
        {
            return _service.GetMonthlyOperations(date, cancellationToken);
        }

        [HttpGet]
        [Route("summary")]
        public Task<List<MonthSummaryDto>> GetSummaryByDateRange([FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate, CancellationToken cancellationToken)
        {
            return _service.GetSummaryByDateRange(startDate, endDate, cancellationToken);
        }

        [HttpPut]
        public override async Task Put(OperationDto operation)
        {
            await _cache.Reset(_cacheKey);
            await _cache.Reset("Accounts");
            await _service.Update(operation);
        }

        [HttpPost]
        public override async Task Post(OperationDto operation)
        {
            await _cache.Reset(_cacheKey);
            await _cache.Reset("Accounts");
            await _service.Create(operation);
        }

        [HttpDelete("{id}")]
        public override async Task Delete(Guid id)
        {
            await _cache.Reset(_cacheKey);
            await _cache.Reset("Accounts");
            await _service.Delete(id);
        }
    }
}
