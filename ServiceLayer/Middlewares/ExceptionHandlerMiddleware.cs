using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ServiceLayer.Middlewares
{
	public class ExceptionHandlerMiddleware(
		ILogger<ExceptionHandlerMiddleware> logger,
		RequestDelegate next)
	{
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				var exceptionId = Guid.NewGuid();
				logger.LogError(ex, "{ErrorId} : {ErrorMessage}", exceptionId, ex.Message);

				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

				var exception = new
				{
					Id = exceptionId,
					Message = "An error occurred while processing the request.",
				};

				await context.Response.WriteAsJsonAsync(exception);
			}
		}
	}
}
