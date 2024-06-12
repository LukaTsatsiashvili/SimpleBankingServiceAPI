using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RepositoryLayer.Context;
using RepositoryLayer.Repositories.Abstract;
using RepositoryLayer.Repositories.Concrete;
using RepositoryLayer.UnitOfWorks.Abstract;
using RepositoryLayer.UnitOfWorks.Concrete;

namespace RepositoryLayer.Extensions
{
	public static class RepositoryLayerExtension
	{
		public static IServiceCollection LoadRepositoryLayerExtension(this IServiceCollection services, IConfiguration config)
		{
			// Add DbContext 
			services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
				config.GetConnectionString("Default")
				?? throw new InvalidOperationException("Connection string is not found!")));

			// Add Generic Repository
			services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

			// Add UnitOfWork
			services.AddScoped<IUnitOfWork, UnitOfWork>();

			return services;
		}
	}
}
