using AutoMapper;
using FP.Application.DTOs;
using FP.Application.Interfaces;
using FP.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : BaseCRUDController<Category, CategoryDto, CategoryValidator>
    {
        public CategoriesController(IRepository<Category> repository, IMapper mapper, ICacheService cache
           ) : base(repository, mapper, cache, "Categories", c => c.Type, c => c.Name)
        {
        }
    }
}
