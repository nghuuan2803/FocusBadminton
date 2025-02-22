using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Data.Repositories
{
    public class CourtRepository : Repository<Court>, IRepository<Court>
    {
        public CourtRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
        public override async Task<IEnumerable<Court>> GetAllAsync(
            Expression<Func<Court, bool>> predicate = null!, 
            CancellationToken cancellationToken = default)
        {
            return predicate == null ? 
                await dbSet.Include(c => c.Facility).ToListAsync(cancellationToken)
                : await dbSet.Include(c => c.Facility).Where(predicate).ToListAsync(cancellationToken);
        }

        public override async Task<Court> FindAsync(Expression<Func<Court, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return (await dbSet.Include(c => c.Facility).FirstOrDefaultAsync(predicate, cancellationToken))!;
        }
    }
}
