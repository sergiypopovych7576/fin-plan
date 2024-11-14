using FP.Application.DTOs;
using FP.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public Task<List<CategoryDto>> Get()
        {
            return _service.Get();
        }
    }
}
