using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace BaseCoreApi.Middelware
{
    public class RateLimitMiddelware
    {
        private readonly RequestDelegate _next;
        //TODO: Get limit from settings. 
        //TODO: Get White list from settings. 
        private const int limit = 5; 
        private readonly IMemoryCache _memoryCashe;
        private readonly ILogger<RateLimitMiddelware> _logger; 

        public RateLimitMiddelware(RequestDelegate next, IMemoryCache memoryCache,ILogger<RateLimitMiddelware> logger )
        {
            _next = next;
            _memoryCashe = memoryCache;
            _logger = logger; 
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var requestKey = context.Connection.RemoteIpAddress;
            int hitCount = 0;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(30)
            };

            if(_memoryCashe.TryGetValue(requestKey, out hitCount))
            {
                if(hitCount < limit)
                {
                    await ProcessRequest(context, requestKey, hitCount, cacheEntryOptions);
                }
                else
                {
                    context.Response.Headers["X-Retry-After"] = cacheEntryOptions.AbsoluteExpiration?.ToString();
                    _logger.LogWarning($"Rete limit for: {requestKey}"); 

                    await context.Response.WriteAsync("Quota exceeded");
                }
            }
            else
            {
                await ProcessRequest(context, requestKey, hitCount, cacheEntryOptions);
            }
        }

        private async Task ProcessRequest(HttpContext context, IPAddress requestKey, int hitCount, MemoryCacheEntryOptions cacheEntryOptions)
        {
            hitCount++;
            _memoryCashe.Set(requestKey, hitCount, cacheEntryOptions); 
            context.Response.Headers["X-Rate-Limit"] = limit.ToString();
            context.Response.Headers["X-Rate-Limit-Remaining"] = (limit - hitCount).ToString();
            await _next(context); 
        }
    }
}
