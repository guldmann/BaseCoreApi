using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace BaseCoreApi.Middelware
{
    public class DemoMiddelware
    {
        private readonly RequestDelegate _next; 

        public DemoMiddelware(RequestDelegate next)
        {
            _next = next; 
        }
        public async Task InvokeAsync(HttpContext context)
        {
            //TODO: bind to object 
            // intercept and alter request if query name == "john"
            var query = context.Request.Query["name"].ToString();
            if (query == "john")
            {
                var qb = new QueryBuilder();
                qb.Add("name", "test");
                qb.Add("age", context.Request.Query["age"].ToString());
                context.Request.QueryString = qb.ToQueryString();
            }

            await _next(context);
        }
    }
}
