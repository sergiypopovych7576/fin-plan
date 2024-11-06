using AutoMapper;
using FP.Application.DTOs;
using FP.Application.Interfaces;
using FP.Application.Services;
using FP.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService service, IMapper mapper)
        {
			_service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<List<CategoryDto>> Get()
        {
            return await _service.Get();
        }
    }
}
