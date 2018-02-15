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
    [Route("api/[controller]")]
    public class PlanetController : Controller
    {
        private readonly IPlanetService planetService;
        private readonly IAppSettings appSettings;
        private readonly ILogger<PlanetController> logger;
        private static Helpers.CircuitBreaker.CircuitBreaker circuitBreaker;

        public PlanetController(IPlanetService planetService, IAppSettings appSettings, ILogger<PlanetController> logger)
        {
            this.planetService = planetService;
            this.appSettings = appSettings;
            this.logger = logger;
            if (circuitBreaker == null)
            {
                circuitBreaker = new Helpers.CircuitBreaker.CircuitBreaker("planet_breaker", appSettings.FailureThreshold, TimeSpan.FromSeconds(appSettings.OpenCircuitTimeout));
            }
        }
        [HttpGet]
        public async Task<IEnumerable<Planet>> Get() => await circuitBreaker.ExecuteAsync(async () => { return await planetService.GetAllPlanetsAsync(); });

        [HttpGet("{id:guid}")]
        public async Task<Planet> Get(Guid id) => await circuitBreaker.ExecuteAsync(async () => { return await planetService.GetPlanetAsync(id); }); 

        [HttpGet("star/{id:guid}")]
        public async Task<IEnumerable<Planet>> Get(Guid id, string star) => await  circuitBreaker.ExecuteAsync(async () => { return await planetService.GetAllPlanetsByStarIdAsync(id); }); 

        [HttpPost]
        public async Task<Planet> Post([FromBody]Planet planet) => await circuitBreaker.ExecuteAsync(async () => { return await planetService.AddPlanetAsync(planet); }); 

        [HttpPost]
        public async Task<IEnumerable<Planet>> Post([FromBody]IEnumerable<Planet> planets) => await circuitBreaker.ExecuteAsync(async () => { return await planetService.AddPlanetsAsync(planets); }); 

        [HttpPut("{id:guid}")]
        public async Task<Planet> Put(Guid id, [FromBody]Planet planet) => await circuitBreaker.ExecuteAsync(async () => { return await planetService.SavePlanetAsync(planet); }); 

        [HttpDelete("{id:guid}")]
        public async Task Delete(Guid id) => await circuitBreaker.ExecuteAsync(async () => { await planetService.DeletePlanetAsync(id); }); 
    }
}