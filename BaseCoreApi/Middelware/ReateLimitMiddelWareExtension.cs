using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCoreApi.Middelware
{
    public static class ReateLimitMiddelWareExtension
    {
        public static IApplicationBuilder UseRateLimit(
           this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitMiddelware>();
        }
    }
}
