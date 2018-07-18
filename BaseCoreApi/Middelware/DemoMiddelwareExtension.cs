using Microsoft.AspNetCore.Builder;

namespace BaseCoreApi.Middelware
{
    public static class DemoMiddelwareExtension
    {
        public static IApplicationBuilder UseDemo(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DemoMiddelware>();
        }
    }
}
