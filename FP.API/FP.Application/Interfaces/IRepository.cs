using FP.Domain.Base;

namespace FP.Application.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        ValueTask<T> GetByIdAsync(Guid id);
        IQueryable<T> GetAll();
        Task<List<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void Update(IEnumerable<T> entities);
        void Remove(T entity);
        Task SaveChangesAsync();
	}
}
