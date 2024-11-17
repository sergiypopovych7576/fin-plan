using AutoMapper;
using FluentValidation;
using FP.Application.Interfaces;
using FP.Domain.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FP.Api.Controllers
{
    public class BaseCRUDController<T, G, V> : ControllerBase where T : BaseEntity, new() where V : AbstractValidator<G>, new()
    {
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private readonly IRepository<T> _repo;
        private readonly AbstractValidator<G> _validator;
        private readonly int _cacheMins = 20;

        protected string _cacheKey;
        protected Expression<Func<T, string>> _orderExpression;

        public BaseCRUDController(IRepository<T> repo, IMapper mapper, ICacheService cache)
        {
            _mapper = mapper;
            _repo = repo;
            _cache = cache;
            _validator = new V();
        }

        [HttpGet]
        public async Task<List<G>> Get(CancellationToken cancellationToken)
        {
            var results = await _cache.Get<List<G>>(_cacheKey);
            if (results == null)
            {
                var query = _repo.GetAll();
                if(_orderExpression != null)
                {
                    query = query.OrderBy(_orderExpression);
                }
                results = await _mapper.ProjectTo<G>(query).ToListAsync(cancellationToken);
            }
            await _cache.Set(_cacheKey, results, _cacheMins);
            return results;
        }

        [HttpPost]
        public async Task Post(G entity)
        {
            await _validator.ValidateAndThrowAsync(entity);
            await _repo.AddAsync(_mapper.Map<T>(entity));
            await _repo.SaveChangesAsync();
            await _cache.Reset(_cacheKey);
        }

        [HttpPut]
        public async Task Put(G entity)
        {
            await _validator.ValidateAndThrowAsync(entity);
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
