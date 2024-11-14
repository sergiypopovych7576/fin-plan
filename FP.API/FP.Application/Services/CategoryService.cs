using AutoMapper;
using FP.Application.DTOs;
using FP.Application.Interfaces;
using FP.Domain;
using Microsoft.EntityFrameworkCore;

namespace FP.Application.Services
{
    public interface ICategoryService : IBaseService
    {
        Task<List<CategoryDto>> Get();
    }

    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _repo;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public CategoryService(IRepository<Category> repo, IMapper mapper, ICacheService cache)
        {
            _repo = repo;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<List<CategoryDto>> Get()
        {
            var cached = await _cache.Get<List<CategoryDto>>("Categories");
            if (cached != null)
                return cached;
            var categories = await _mapper.ProjectTo<CategoryDto>(_repo.GetAll().OrderBy(c => c.Type).ThenBy(c => c.Name).AsNoTracking()).ToListAsync();
            await _cache.Set("Categories", categories, 60);
            return categories;
        }
    }
}
