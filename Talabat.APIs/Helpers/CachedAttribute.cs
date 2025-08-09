using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using Talabat.Core.Services;

namespace Talabat.APIs.Helpers
{
	public class CachedAttribute : Attribute, IAsyncActionFilter
	{
		private readonly int _timeToLiveSeconds;

		public CachedAttribute(int timeToLiveSeconds)
        {
			_timeToLiveSeconds = timeToLiveSeconds;
		}
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

			var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

			var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);

			if (!string.IsNullOrEmpty(cachedResponse))
			{
				var contentResult = new ContentResult()
				{
					Content = cachedResponse,
					ContentType = "application/json",
					StatusCode = 200
				};
				context.Result = contentResult;

				return; // Don't Execute The Endpoint
			}

			var executedEndpointContext = await next(); // Execute The Endpoint If There Is No Another Action Filters After This

			if (executedEndpointContext.Result is OkObjectResult okObjectResult) // If It Is OkObjectResult Name This Result okObjectResult
			{
				await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds)); // okObjectResult.Value -> Response [Data] ItSelf
			}
		}

		private string GenerateCacheKeyFromRequest(HttpRequest request)
		{
			StringBuilder keyBuilder = new StringBuilder();

			keyBuilder.Append(request.Path); // => Url Path e.g. /api/products
		
			foreach (var (key, value) in request.Query.OrderBy(pair => pair.Key))
			{
				keyBuilder.Append($"|{key}-{value}");
			}
			// => cacheKey = /api/products|pageIndex-1||pageSize-5|sort-name

			return keyBuilder.ToString();
		}
	}
}
