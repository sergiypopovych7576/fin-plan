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
        protected readonly IMapper _mapper;
        protected readonly ICacheService _cache;
        protected readonly IRepository<T> _repo;
        protected readonly AbstractValidator<G> _validator;
        protected readonly int _cacheMins = 20;

        protected string _cacheKey;
        protected Expression<Func<T, object>> _orderExpression;
        protected Expression<Func<T, object>> _thenOrderExpression;

        public BaseCRUDController(
        IRepository<T> repo,
        IMapper mapper,
        ICacheService cache,
        string cacheKey,
        Expression<Func<T, object>> orderExpression,
        Expression<Func<T, object>> thenOrderExpression = null)
        {
            _mapper = mapper;
            _repo = repo;
            _cache = cache;
            _validator = new V();
            _cacheKey = cacheKey ?? throw new ArgumentNullException(nameof(cacheKey));
            _orderExpression = orderExpression ?? throw new ArgumentNullException(nameof(orderExpression));
            _thenOrderExpression = thenOrderExpression;
        }

        [HttpGet]
        public virtual async Task<List<G>> Get(CancellationToken cancellationToken)
        {
            var results = await _cache.Get<List<G>>(_cacheKey);
            if (results == null)
            {
                var query = _repo.GetAll();
                if (_orderExpression != null)
                {
                    if (_thenOrderExpression == null)
                    {
                        query = query.OrderBy(_orderExpression);
                    }
                    else
                    {
                        query = query.OrderBy(_orderExpression).ThenBy(_thenOrderExpression);
                    }
                }
                results = await _mapper.ProjectTo<G>(query).ToListAsync(cancellationToken);
                await _cache.Set(_cacheKey, results, _cacheMins);
            }
            return results;
        }

        [HttpPost]
        public virtual async Task Post(G entity)
        {
            await _validator.ValidateAndThrowAsync(entity);
            await _repo.AddAsync(_mapper.Map<T>(entity));
            await _repo.SaveChangesAsync();
            await _cache.Reset(_cacheKey);
        }

        [HttpPut]
        public virtual async Task Put(G entity)
        {
            await _validator.ValidateAndThrowAsync(entity);
            _repo.Update(_mapper.Map<T>(entity));
            await _repo.SaveChangesAsync();
            await _cache.Reset(_cacheKey);
        }

        [HttpDelete("{id}")]
        public virtual async Task Delete(Guid id)
        {
            _repo.Remove(new T { Id = id });
            await _repo.SaveChangesAsync();
            await _cache.Reset(_cacheKey);
        }
    }
}
