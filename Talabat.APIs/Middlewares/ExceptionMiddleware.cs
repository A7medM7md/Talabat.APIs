using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Middlewares
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate next;
		private readonly ILogger<ExceptionMiddleware> logger;
		private readonly IHostEnvironment env;

		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
			this.next = next;
			this.logger = logger;
			this.env = env;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await next.Invoke(context); // No Exceptions, Go To Next Middleware
			}
			catch(Exception ex)
			{
				logger.LogError(ex, ex.Message); // Log Exception On Console Screen [Development Env]
												 // If you are In Production : Use Log Package To Log The Exception In DB

				// Determine Headers Info [ContentTType & StatusCode]
				context.Response.Headers.ContentType = "application/json";
				var statusCode = context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

				// Handle Response
				var response = env.IsDevelopment()?
					new ApiExceptionResponse(statusCode, ex.Message, ex.StackTrace.ToString())
					: new ApiExceptionResponse(statusCode, ex.Message, ex.StackTrace.ToString());

				var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

				var json = JsonSerializer.Serialize(response, options);

				await context.Response.WriteAsync(json);
            }
        }

    }
}
