using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net;
using System.Threading.Tasks;
using BaseCoreApi.Models; 

namespace BaseCoreApi.Middelware
{
    public class RateLimitMiddelware
    {        
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _memoryCashe;
        private readonly ILogger<RateLimitMiddelware> _logger;
        private  RateLimitOptions options;

        public RateLimitMiddelware(RequestDelegate next, IMemoryCache memoryCache,ILogger<RateLimitMiddelware> logger)
        {                                                                  
            _next = next;
            _memoryCashe = memoryCache;
            _logger = logger;           
        }
        public async Task InvokeAsync(HttpContext context, IOptionsSnapshot<RateLimitOptions> rateLimitOptions)
        {            
            options = rateLimitOptions.Value; 
            var requestKey = context.Connection.RemoteIpAddress;
            int hitCount = 0;            

            var cacheEntryOptions = new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddSeconds(options.TimeSec)
            };

            if(_memoryCashe.TryGetValue(requestKey, out hitCount))
            {
                if(hitCount < options.Limit)
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
            if (Array.IndexOf(options.WhiteList, requestKey.ToString()) >= 0)
            {
                context.Response.Headers["X-Rate-Limit"] = "No-limit"; 
                context.Response.Headers["X-Rate-Limit-Remaining"] = "No-limit";
                await _next(context);
            }
            else
            {
                hitCount++;
                _memoryCashe.Set(requestKey, hitCount, cacheEntryOptions);
                context.Response.Headers["X-Rate-Limit"] = options.Limit.ToString();
                context.Response.Headers["X-Rate-Limit-Remaining"] = (options.Limit - hitCount).ToString();
                await _next(context);
            }
        }
    }
}
