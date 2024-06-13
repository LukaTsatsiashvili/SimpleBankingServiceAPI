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

		public bool DeleteEntity(T entity)
		{
			var result = _dbSet.Remove(entity);
			if (result != null) return true;

			return false;
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

	}
}
