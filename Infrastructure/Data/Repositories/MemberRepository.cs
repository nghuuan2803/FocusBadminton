using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Data.Repositories
{
    public class MemberRepository : Repository<Member>, IRepository<Member>
    {
        public MemberRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IEnumerable<Member>> GetAllAsync(Expression<Func<Member, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
            {
                return await dbSet.Include(p => p.Account).ToListAsync(cancellationToken);
            }
            else
            {
                return await dbSet.Where(predicate).Include(p => p.Account).ToListAsync(cancellationToken);
            }
        }

        public override async Task<Member> FindAsync(object id, CancellationToken cancellationToken = default)
        {
            return await dbSet.Where(p => p.Id == (int)id).Include(p => p.Account).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
