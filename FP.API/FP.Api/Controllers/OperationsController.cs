using FP.Application.DTOs;
using FP.Application.Services;
using FP.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FP.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OperationsController : ControllerBase
	{
		private readonly IOperationsService _opsService;
		public OperationsController(IOperationsService opsService) {
			_opsService = opsService;
		}

		[HttpGet]
		public Task<List<OperationDto>> Get([FromQuery] int year, [FromQuery] int month, CancellationToken cancellationToken) {
			return _opsService.GetMonthlyOperations(year, month, cancellationToken);
		}

		[HttpPost]
		public Task Post(OperationDto operation) {
			return _opsService.Create(operation);
		}

        [HttpPost]
        [Route("sync")]
        public Task Sync()
        {
            return _opsService.Sync();
        }

        [HttpDelete("{id}")]
		public Task Delete(Guid id) {
			return _opsService.Delete(id);
		}
	}
}
