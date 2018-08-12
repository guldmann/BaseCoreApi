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
using BaseCoreApi.Middelware;
using System.Reflection;
using BaseCoreApi.Settings;
using BaseCoreApi.Data;
using Microsoft.EntityFrameworkCore;

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
                     config.Path = "Settings/appsettings.json";
                     config.ReloadOnChange = true;
                 })
                 
                 .AddJsonFile(config =>
                 {
                     config.Path = "Settings/appsettingsSerilog.json";
                     config.ReloadOnChange = true;
                 })
                 .AddEnvironmentVariables()
                 .AddInMemoryCollection();
            Configuration = builder.Build();


            //Log to Elasticsearch on localhost / Kiban  And to Rolling text file 
            //TODO: Log settings will not reload when settings change 
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.ControlledBy(new VariableLoggingLevelSwitch(Configuration["Serilog:LogLevel"]))
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(Configuration["Elasticsearch:Uri"]))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
                })
                .WriteTo.RollingFile(Configuration["Serilog:File"])
                .CreateLogger();

           //WIP to read serilog settings from AppsettingsSerilog.json
          //var logger = new LoggerConfiguration()
          //    .ReadFrom.Configuration(Configuration)
          //    .CreateLogger();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //TODO Work in progress
            //TODO: if development rebuild database at startup and add some data.
            //EF Database
            var connectionString = Configuration.GetConnectionString("localdb");
            services.AddDbContext<PersonContext>(
                options => options.UseSqlServer(connectionString));

            //Add settings poco classes to DI 
            services.Configure<RateLimitOptions>(Configuration.GetSection("RateLimit"));
            services.Configure<JWTOptions>(Configuration.GetSection("Jwt"));
                       
            //JWT Authentication
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var serverSecret = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["Jwt:ServerSecret"]));
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        IssuerSigningKey = serverSecret,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"]
                    };                   
                });

            //Add MVC
            services.AddMvc();

            //Memory cache for RateLimit
            services.AddMemoryCache(); 

            //Add personservice class as singleton
           // services.AddSingleton<IPersonService,PersonService>();
           //Changed from singelton to scoped for dbcontext to work 
            services.AddScoped<IPersonService, PersonService>();

            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "BaseCoreApi API", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory )
        {
            //Logging 
            loggerFactory.AddDebug();
            loggerFactory.AddConsole();
            loggerFactory.AddSerilog();

           

            //DEV MODE 
            if (env.IsDevelopment())
            {
                
                //Add header when in developer mode 
                app.Use(async(context,next) =>
                {
                    context.Response.Headers["X-Environment-name"] = env.EnvironmentName;
                    await next();
                });
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage(); 

            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            
            //Authentication
            app.UseAuthentication();

            //Middle-wares only used when path starts with  "/api/"
            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/"), appBuilder =>
            {
                app.UseRateLimit();
                appBuilder.UseDemo();
            });

            //Swagger 
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "BaseCoreApi API V1");
            });

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
