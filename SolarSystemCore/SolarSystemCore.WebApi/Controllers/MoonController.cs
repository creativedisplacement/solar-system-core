using Microsoft.AspNetCore.Mvc;
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
        private static Helpers.CircuitBreaker.CircuitBreaker circuitBreaker;
        private readonly IAppSettings appSettings;

        public MoonController(IMoonService moonService, IAppSettings appSettings)
        {
            this.moonService = moonService;
            this.appSettings = appSettings;
            if (circuitBreaker == null)
            {
                circuitBreaker = new Helpers.CircuitBreaker.CircuitBreaker("moon_breaker", appSettings.FailureThreshold, TimeSpan.FromSeconds(appSettings.OpenCircuitTimeout));
            }
        }

        [HttpGet]
        public async Task<IEnumerable<Moon>> Get() => await circuitBreaker.ExecuteAsync(async () => { return await moonService.GetAllMoonsAsync(); }); 

        [HttpGet("{id:int}")]
        public async Task<Moon> Get(int id) => await circuitBreaker.ExecuteAsync(async () => { return await moonService.GetMoonAsync(id); }); 

        [HttpGet("planet/{id:int}")]
        public async Task<IEnumerable<Moon>> Get(int id, string star) => await circuitBreaker.ExecuteAsync(async () => { return await moonService.GetAllMoonsByPlanetIdAsync(id); }); 

        [HttpPost]
        public async Task<Moon> Post([FromBody]Moon moon) => await circuitBreaker.ExecuteAsync(async () => { return await moonService.AddMoonAsync(moon); }); 

        [HttpPost]
        public async Task<IEnumerable<Moon>> Post([FromBody]IEnumerable<Moon> moons) => await circuitBreaker.ExecuteAsync(async () => { return await moonService.AddMoonsAsync(moons); }); 

        [HttpPut("{id:int}")]
        public async Task<Moon> Put(int id, [FromBody]Moon moon) => await circuitBreaker.ExecuteAsync(async () => { return await moonService.SaveMoonAsync(moon); }); 

        [HttpDelete("{id:int}")]
        public async Task Delete(int id) => await circuitBreaker.ExecuteAsync(async () => { await moonService.DeleteMoonAsync(id); }); 
    }
}