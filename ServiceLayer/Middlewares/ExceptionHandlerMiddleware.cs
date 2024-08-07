using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace ServiceLayer.Middlewares
{
	public class ExceptionHandlerMiddleware(
		ILogger<ExceptionHandlerMiddleware> logger,
		RequestDelegate next)
	{

		public async Task InvokeAsync(HttpContext context)
		{
			// Declare variables
			string title = "Warrning";
			string message = "Internal Server Error";
			int statusCode = (int)HttpStatusCode.InternalServerError;

			try
			{
				await next(context);

				// If response is Unauthorized - 401 
				if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
				{
					title = "Unauthorized";
					message = "Unauthorized request! Please authenticate.";
					statusCode = (int)HttpStatusCode.Unauthorized;
					
					await ModifyHeader(context, title, message, statusCode);
				}

				// If response is Forbidden - 403
				if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
				{
					title = "Forbidden";
					message = "Forbidden request!";
					statusCode = (int)HttpStatusCode.Forbidden;

					await ModifyHeader(context, title, message, statusCode);
				}
			}
			catch (Exception ex)
			{
				var exceptionId = Guid.NewGuid();
				logger.LogError(ex, "{ErrorId} : {ErrorMessage}", exceptionId, ex.Message);


				if (ex is TaskCanceledException || ex is TimeoutException)
				{
					title = "Out of time!";
					message = "Request timeout ... Try again later.";
					statusCode = (int)HttpStatusCode.RequestTimeout;
				}

				await ModifyHeader(context, title, message, statusCode);
			}
		}

		private static async Task ModifyHeader(HttpContext context, string title, string message, int statusCode)
		{
			context.Response.ContentType = "application/json";

			await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
			{
				Detail = message,
				Status = statusCode,
				Title = title
			}),CancellationToken.None);
		}
	}
}
