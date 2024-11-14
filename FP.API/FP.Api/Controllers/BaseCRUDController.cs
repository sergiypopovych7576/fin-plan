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
        private readonly IRepository<T> _repo;

        public BaseCRUDController(IRepository<T> repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public Task<List<G>> Get(CancellationToken cancellationToken)
        {
            return _mapper.ProjectTo<G>(_repo.GetAll()).ToListAsync(cancellationToken);
        }

        [HttpPost]
        public Task Post(G entity)
        {
            _repo.AddAsync(_mapper.Map<T>(entity));
            return _repo.SaveChangesAsync();
        }

        [HttpPut]
        public Task Put(G entity)
        {
            _repo.Update(_mapper.Map<T>(entity));
            return _repo.SaveChangesAsync();
        }

        [HttpDelete("{id}")]
        public Task Delete(Guid id)
        {
            _repo.Remove(new T { Id = id });
            return _repo.SaveChangesAsync();
        }
    }
}
