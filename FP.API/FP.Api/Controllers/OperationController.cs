using AutoMapper;
using FP.Application.DTOs;
using FP.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationController : ControllerBase
    {
        private readonly IOperationsService _opsService;
        public OperationController(IOperationsService opsService)
        {
            _opsService = opsService;
        }

        [HttpGet("year/{year}/month/{month}")]
        public Task<List<OperationDto>> Get([FromRoute] int year, [FromRoute] int month)
        {
            return _opsService.GetMonthlyOperations(year, month);
        }
    }
}
