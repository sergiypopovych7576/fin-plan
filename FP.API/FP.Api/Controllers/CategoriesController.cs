using AutoMapper;
using FP.Application.DTOs;
using FP.Application.Interfaces;
using FP.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : BaseCRUDController<Category, CategoryDto>
    {
        public CategoriesController(IRepository<Category> repository, IMapper mapper) : base(repository, mapper) { }
    }
}
