using RepositoryLayer.Context;
using RepositoryLayer.Repositories.Abstract;
using RepositoryLayer.Repositories.Concrete;
using RepositoryLayer.UnitOfWorks.Abstract;

namespace RepositoryLayer.UnitOfWorks.Concrete
{
	public class UnitOfWork(AppDbContext context) : IUnitOfWork
	{
		public IGenericRepository<T> GetGenericRepository<T>() where T : class
		{
			return new GenericRepository<T>(context);
		}

		public void Save()
		{
			context.SaveChanges();
		}

		public async Task SaveAsync()
		{
			await context.SaveChangesAsync();
		}
	}
}
