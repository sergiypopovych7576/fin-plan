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
        public Task Sync()
        {
            return _service.Sync();
        }

        [HttpGet]
        [Route("month/{date}")]
        public Task<List<OperationDto>> Get([FromRoute] DateOnly date, CancellationToken cancellationToken)
        {
            return _service.GetMonthlyOperations(date, cancellationToken);
        }

        [HttpPost]
        public override Task Post(OperationDto operation)
        {
            return _service.Create(operation);
        }

        [HttpDelete("{id}")]
        public override Task Delete(Guid id)
        {
            return _service.Delete(id);
        }
    }
}
