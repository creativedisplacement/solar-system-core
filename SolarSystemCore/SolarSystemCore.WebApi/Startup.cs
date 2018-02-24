using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using SolarSystemCore.Core;
using SolarSystemCore.Data;
using SolarSystemCore.Repositories;
using SolarSystemCore.Services;
using StructureMap;

namespace SolarSystemCore.WebApi
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
            .AddEnvironmentVariables();
            _configuration = builder.Build();
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(_configuration).CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DBContext>(options => options.UseSqlServer(_configuration["Data:ConnectionString"]));
            services.AddSingleton<IConfiguration>(_configuration);
            services.AddSingleton<IAppSettings>(_configuration.GetSection("AppSettings").Get<AppSettings>());
            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
        }

        // Use StructureMap-specific APIs to register services in the registry.
        public void ConfigureContainer(Registry registry)
        {
            registry.Scan(register =>
            {
                register.Assembly("SolarSystemCore.Core");
                register.Assembly("SolarSystemCore.Repositories");
                register.Assembly("SolarSystemCore.Services");
                register.WithDefaultConventions();
                register.AddAllTypesOf(typeof(IRepository<>));
                register.ConnectImplementationsToTypesClosing(typeof(IRepository<>)).OnAddedPluginTypes(c => c.ContainerScoped());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory, 
            IApplicationLifetime appLifetime, 
            DBContext context)
        {
            loggerFactory.AddSerilog();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            SeedData.Initialize(context);
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
        }
    }
}
