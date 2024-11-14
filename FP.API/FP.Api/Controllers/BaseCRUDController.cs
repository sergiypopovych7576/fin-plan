using AutoMapper;
using FP.Application.Interfaces;
using FP.Domain.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FP.Api.Controllers
{
    public class BaseCRUDController<T, G> : ControllerBase where T : BaseEntity, new()
    {
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly IRepository<T> _repo;

        protected string _cacheKey;

        public BaseCRUDController(IRepository<T> repo, IMapper mapper, ICacheService cache)
        {
            _mapper = mapper;
            _repo = repo;
            _cache = cache;
        }

        [HttpGet]
        public async Task<List<G>> Get(CancellationToken cancellationToken)
        {
            var results = await _cache.Get<List<G>>(_cacheKey);
            if (results == null)
            {
                results = await _mapper.ProjectTo<G>(_repo.GetAll()).ToListAsync(cancellationToken);
            }
            await _cache.Set(_cacheKey, results, 20);
            return results;
        }

        [HttpPost]
        public async Task Post(G entity)
        {
            await _repo.AddAsync(_mapper.Map<T>(entity));
            await _repo.SaveChangesAsync();
            await _cache.Reset(_cacheKey);
        }

        [HttpPut]
        public async Task Put(G entity)
        {
            _repo.Update(_mapper.Map<T>(entity));
            await _repo.SaveChangesAsync();
            await _cache.Reset(_cacheKey);
        }

        [HttpDelete("{id}")]
        public async Task Delete(Guid id)
        {
            _repo.Remove(new T { Id = id });
            await _repo.SaveChangesAsync();
            await _cache.Reset(_cacheKey);
        }
    }
}
