using RepositoryLayer.Repositories.Abstract;

namespace RepositoryLayer.UnitOfWorks.Abstract
{
	public interface IUnitOfWork
	{
		void Save();
		Task SaveAsync();
		IGenericRepository<T> GetGenericRepository<T>() where T : class;


		Task BeginTransactionAsync();
		Task CommitTransactionAsync();
		Task RollbackTransactionAsync();
	}
}
