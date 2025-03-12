using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Data.Repositories
{
    public class BookingRepository : Repository<Booking>, IRepository<Booking>
    {
        public BookingRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IEnumerable<Booking>> GetAllAsync(Expression<Func<Booking, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                return await dbSet.Include(b => b.Member)
                    .Include(p => p.Team)
                    .Include(p => p.Voucher)
                    .Include(p => p.Promotion)
                    .Include(p => p.Details)
                        .ThenInclude(d => d.Court)
                    .ToListAsync(cancellationToken);

            return await dbSet.Include(b => b.Member).Where(predicate)
                .Include(p => p.Team)
                .Include(p => p.Voucher)
                .Include(p => p.Promotion)
                .Include(p => p.Details)
                    .ThenInclude(d => d.Court)
                .ToListAsync(cancellationToken);
        }

        public override async Task<Booking> FindAsync(Expression<Func<Booking, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await dbSet.Include(b => b.Member)
                .Include(p => p.Team)
                .Include(p => p.Voucher)
                .Include(p => p.Promotion)
                .Include(p => p.Details)
                    .ThenInclude(d => d.Court)
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }
    }
}
