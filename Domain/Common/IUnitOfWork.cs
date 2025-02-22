namespace Domain.Common
{
    public interface IUnitOfWork
    {
        Task BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task SaveAsync();
        TRepository GetRepository<TRepository>()
        where TRepository : class;
    }
}
