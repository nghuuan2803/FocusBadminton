using Domain.Common;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private IDbContextTransaction _transaction = null!;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(IServiceProvider serviceProvider, AppDbContext appDbContext)
        {
            _serviceProvider = serviceProvider;
            _dbContext = appDbContext;
        }

        public async Task BeginAsync()
        {
            _transaction = await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction == null)
                throw new InvalidOperationException("Transaction has not been started.");

            try
            {
                await _dbContext.SaveChangesAsync();
                await _transaction.CommitAsync();
                DisposeTransaction();
            }
            catch (Exception)
            {
                await RollbackAsync();
                throw;
            }

        }
        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                DisposeTransaction();
            }
        }

        public Task SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }
        private void DisposeTransaction()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null!;
            }
        }

        public TRepository GetRepository<TRepository>() where TRepository : class
        {
            var type = typeof(TRepository);

            if (!_repositories.TryGetValue(type, out var repository))
            {
                repository = _serviceProvider.GetRequiredService<TRepository>();
                _repositories[type] = repository;
            }

            return (TRepository)repository;
        }
        public void Dispose()
        {
            DisposeTransaction();
            _dbContext.Dispose();
            _repositories.Clear();
        }
    }
}
