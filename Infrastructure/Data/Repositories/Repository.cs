using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;
        protected readonly DbSet<T> dbSet;

        public Repository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            dbSet = dbContext.Set<T>();
        }

        public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        }

        public virtual Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            return _dbContext.Set<T>().AddRangeAsync(entities, cancellationToken);
        }

        public virtual Task<T> FindAsync(Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken)!;
        }

        public virtual async Task<T> FindAsync(object id,
            CancellationToken cancellationToken = default)
        {
            return (await _dbContext.Set<T>().FindAsync(id, cancellationToken))!;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate = null!,
            CancellationToken cancellationToken = default)
        {
            return await (predicate == null ?
                _dbContext.Set<T>().ToListAsync(cancellationToken) :
                _dbContext.Set<T>().Where(predicate).ToListAsync(cancellationToken));
        }

        public virtual void Remove(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public virtual async Task<bool> RemoveByIdAsync(object id,
            CancellationToken cancellationToken = default)
        {
            T entity = (await _dbContext.Set<T>().FindAsync(id, cancellationToken))!;
            if (entity == null)
                return false;
            _dbContext.Set<T>().Remove(entity);
            return true;
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        public Task SaveAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            dbSet.UpdateRange(entities);
        }
    }
}
