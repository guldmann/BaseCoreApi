using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseCoreApi.Models;
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
            var name = context.Request.Query["name"].ToString();
            int age;
            Int32.TryParse(context.Request.Query["age"], out age);

            Person person = new Person { Name = name, Age = age }; 

            // Intercept and alter request name if person name == "john"
            if(!string.IsNullOrEmpty(person.Name) && person.Name == "john")
            {
                var newQuery = new QueryBuilder();
                newQuery.Add("name", "new test name");
                newQuery.Add("age", person.Age.ToString());
                context.Request.QueryString = newQuery.ToQueryString();
            }

            await _next(context);
        }
    }
}
