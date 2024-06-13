using System.Linq.Expressions;

namespace RepositoryLayer.Repositories.Abstract
{
	public interface IGenericRepository<T> where T : class
	{
		IQueryable<T> GetAllEntityAsync();
		Task<T> GetEntityByIdAsync(Guid id);
		Task AddEntityAsync(T entity);
		void UpdateEntity(T entity);
		bool DeleteEntityAsync(T entity);
		IQueryable<T> Where(Expression<Func<T, bool>> predicate);

	}
}
