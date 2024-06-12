using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using RepositoryLayer.Repositories.Abstract;
using System.Linq.Expressions;

namespace RepositoryLayer.Repositories.Concrete
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly AppDbContext _context;
		private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
			_context = context;
			_dbSet = _context.Set<T>();
        }


        public async Task AddEntityAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public void DeleteEntity(T entity)
		{
			_dbSet.Remove(entity);
		}

		public IQueryable<T> GetAllEntityAsync()
		{
			return _dbSet.AsNoTracking().AsQueryable();
		}

		public async Task<T> GetEntityByIdAsync(Guid id)
		{
			return await _dbSet.FindAsync(id);
		}

		public void UpdateEntity(T entity)
		{
			_dbSet.Update(entity);
		}

		public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
		{
			return _dbSet.Where(predicate);
		}

		public async Task BeginTransactionAsync()
		{
			await _context.Database.BeginTransactionAsync();
		}

		public async Task CommitTransactionAsync()
		{
			await _context.Database.CommitTransactionAsync();
		}

		public async Task RollbackTransactionAsync()
		{
			await _context.Database.RollbackTransactionAsync();
		}
	}
}
