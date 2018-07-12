using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using BaseCoreApi.Models;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
//using Serilog.Sinks.LogstashHttp;

namespace BaseCoreApi
{
    public class Startup
    {        

        public Startup(IHostingEnvironment env)
        {
            // Load appsettings
            var builder = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile(config =>
                 {
                     config.Path = "jwtSettings.json";
                     config.ReloadOnChange = true;
                 })
                 .AddJsonFile("appsettings.json")
                 .AddEnvironmentVariables()
                 .AddInMemoryCollection();

            JWTOptions jWTOptions = new JWTOptions();
            builder.Build().Bind(jWTOptions);
            Configuration = builder.Build();

            //Log to Elasticsearch on localhost / Kiban  And to Rolling text file 
            //TODO: Get log-level and Elasticsearch Uri from configuration 
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()                
                .WriteTo.RollingFile(Path.Combine(env.ContentRootPath, "BaseCoreApi-log-{Date}.log")) //in tamplate make this like "$safeprojectname$-log-{Date}.log"               
                .WriteTo.Elasticsearch().WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://127.0.0.1:9200"))
                {
                    MinimumLogEventLevel = LogEventLevel.Debug,
                    AutoRegisterTemplate = true,
                })
                .CreateLogger();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            //JWT Authentication 
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var serverSecret = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["ServerSecret"]));
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = serverSecret,
                        ValidIssuer = Configuration["Issuer"],
                        ValidAudience = Configuration["Audience"]
                    };
                });

            //JWT settings 
            services.Configure<JWTOptions>(Configuration);

            //MVC and POCO classes 
            services.AddMvc();
            services.AddSingleton<IPersonService,PersonService>(); 

            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "BaseCoreApi API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //Logging 
            loggerFactory.AddDebug();
            loggerFactory.AddConsole();
            loggerFactory.AddSerilog(); 

            //DEV MODE 
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //Swagger 
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BaseCoreApi API V1");
            });

            //Authentication
            app.UseAuthentication(); 

            //MVC
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
