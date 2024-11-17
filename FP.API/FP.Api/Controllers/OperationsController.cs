using FP.Application.DTOs;
using FP.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly IOperationsService _service;

        public OperationsController(IOperationsService service)
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
        public Task<List<OperationDto>> Get([FromQuery] DateOnly date, CancellationToken cancellationToken)
        {
            return _service.GetMonthlyOperations(date, cancellationToken);
        }

        [HttpPost]
        public Task Post(OperationDto operation)
        {
            return _service.Create(operation);
        }

        [HttpDelete("{id}")]
        public Task Delete(Guid id)
        {
            return _service.Delete(id);
        }
    }
}
