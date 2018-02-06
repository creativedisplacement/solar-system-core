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
using Serilog;
using SolarSystemCore.Core;
using SolarSystemCore.Data;
using SolarSystemCore.Repositories;
using SolarSystemCore.Services;

namespace SolarSystemCore.WebApi
{
    public class Startup
    {
        private IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
            _configuration = builder.Build();
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(_configuration).CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DBContext>(options => options.UseSqlServer(_configuration["Data:ConnectionString"]));
            services.AddSingleton<IConfiguration>(_configuration);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IStarService, StarService>();
            services.AddScoped<IPlanetService, PlanetService>();
            services.AddScoped<IMoonService, MoonService>();
            services.AddSingleton<IAppSettings>(_configuration.GetSection("AppSettings").Get<AppSettings>());
            services.AddMvc();
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
