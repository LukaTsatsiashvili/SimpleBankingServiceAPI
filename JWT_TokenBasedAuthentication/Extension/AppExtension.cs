using Serilog;
using ServiceLayer.Middlewares;

namespace JWT_TokenBasedAuthentication.Extension
{
	public static class AppExtension
	{
		public static void SerilogConfiguration(this IHostBuilder host)
		{
			host.UseSerilog((context, loggerConfig) =>
			{
				loggerConfig.ReadFrom.Configuration(context.Configuration);
			});
		}		
	}
}
