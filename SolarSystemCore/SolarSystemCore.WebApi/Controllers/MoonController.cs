using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarSystemCore.Core;
using SolarSystemCore.Models;
using SolarSystemCore.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolarSystemCore.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Moon")]
    public class MoonController : Controller
    {
        private readonly IMoonService moonService;
        private readonly IAppSettings appSettings;
        private readonly ILogger<MoonController> logger;
        private static Helpers.CircuitBreaker.CircuitBreaker circuitBreaker;

        public MoonController(IMoonService moonService, IAppSettings appSettings, ILogger<MoonController> logger)
        {
            this.moonService = moonService;
            this.appSettings = appSettings;
            this.logger = logger;
            if (circuitBreaker == null)
            {
                circuitBreaker = new Helpers.CircuitBreaker.CircuitBreaker("moon_breaker", appSettings.FailureThreshold, TimeSpan.FromSeconds(appSettings.OpenCircuitTimeout));
            }
        }

        [HttpGet]
        public async Task<IEnumerable<Moon>> Get() => await circuitBreaker.ExecuteAsync(async () => { return await moonService.GetAllMoonsAsync(); }); 

        [HttpGet("{id:guid}")]
        public async Task<Moon> Get(Guid id) => await circuitBreaker.ExecuteAsync(async () => { return await moonService.GetMoonAsync(id); }); 

        [HttpGet("planet/{id:guid}")]
        public async Task<IEnumerable<Moon>> Get(Guid id, string star) => await circuitBreaker.ExecuteAsync(async () => { return await moonService.GetAllMoonsByPlanetIdAsync(id); }); 

        [HttpPost]
        public async Task<Moon> Post([FromBody]Moon moon) => await circuitBreaker.ExecuteAsync(async () => { return await moonService.AddMoonAsync(moon); }); 

        [HttpPost]
        public async Task<IEnumerable<Moon>> Post([FromBody]IEnumerable<Moon> moons) => await circuitBreaker.ExecuteAsync(async () => { return await moonService.AddMoonsAsync(moons); }); 

        [HttpPut("{id:guid}")]
        public async Task<Moon> Put(Guid id, [FromBody]Moon moon) => await circuitBreaker.ExecuteAsync(async () => { return await moonService.SaveMoonAsync(moon); }); 

        [HttpDelete("{id:guid}")]
        public async Task Delete(Guid id) => await circuitBreaker.ExecuteAsync(async () => { await moonService.DeleteMoonAsync(id); }); 
    }
}