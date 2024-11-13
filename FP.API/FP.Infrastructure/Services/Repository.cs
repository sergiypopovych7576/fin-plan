using FP.Application.Interfaces;
using FP.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace FP.Infrastructure.Services
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly BudgetDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(BudgetDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public ValueTask<T> GetByIdAsync(Guid id)
        {
            return _dbSet.FindAsync(id);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public Task<List<T>> GetAllAsync()
        {
            return _dbSet.AsQueryable().ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Update(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
