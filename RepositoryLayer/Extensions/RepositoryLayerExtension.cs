using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RepositoryLayer.Context;

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
			
			return services;
		}
	}
}
