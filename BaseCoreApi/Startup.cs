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
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Serilog.Configuration;
using System;
//using Serilog.Sinks.LogstashHttp;

namespace BaseCoreApi
{
    public class Startup
    {        

        public Startup(IHostingEnvironment env)
        {
            // Load Settings
            var builder = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile(config =>
                 {
                     config.Path = "jwtSettings.json";
                     config.ReloadOnChange = true;
                 })
                 .AddJsonFile("appsettings.json")
                 .AddJsonFile("appsettingsSerilog.json")
                 .AddEnvironmentVariables()
                 .AddInMemoryCollection();

            //Add jwtOption to configuration 
            JWTOptions jWTOptions = new JWTOptions();
            builder.Build().Bind(jWTOptions);
            Configuration = builder.Build();        

            //Log to Elasticsearch on localhost / Kiban  And to Rolling text file 
            //TODO: Get log-level and Elasticsearch Uri from configuration             
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
                })
                .WriteTo.RollingFile("BaseCoreApi-log-{Date}.log")     
                .CreateLogger();
                


            /*
             * Work in progress to read serilog settings from AppsettingsSerilog.json 
             * 
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            */

        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.ConfigureApplicationCookie(option => option.LoginPath = "/Authenticate/");
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

            //Demo middleware 
            //TODO: Add demo middleware here

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
