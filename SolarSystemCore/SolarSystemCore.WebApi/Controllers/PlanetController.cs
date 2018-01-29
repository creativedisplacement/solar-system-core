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
    [Route("api/[controller]")]
    public class PlanetController : Controller
    {
        private readonly IPlanetService planetService;
        private static Helpers.CircuitBreaker.CircuitBreaker circuitBreaker;
        private readonly IAppSettings appSettings;

        public PlanetController(IPlanetService planetService, IAppSettings appSettings)
        {
            this.planetService = planetService;
            this.appSettings = appSettings;
            if (circuitBreaker == null)
            {
                circuitBreaker = new Helpers.CircuitBreaker.CircuitBreaker("planet_breaker", appSettings.FailureThreshold, TimeSpan.FromSeconds(appSettings.OpenCircuitTimeout));
            }
        }
        [HttpGet]
        public async Task<IEnumerable<Planet>> Get() => await circuitBreaker.ExecuteAsync(async () => { return await planetService.GetAllPlanetsAsync(); });

        [HttpGet("{id:int}")]
        public async Task<Planet> Get(int id) => await circuitBreaker.ExecuteAsync(async () => { return await planetService.GetPlanetAsync(id); }); 

        [HttpGet("star/{id:int}")]
        public async Task<IEnumerable<Planet>> Get(int id, string star) => await  circuitBreaker.ExecuteAsync(async () => { return await planetService.GetAllPlanetsByStarIdAsync(id); }); 

        [HttpPost]
        public async Task<Planet> Post([FromBody]Planet planet) => await circuitBreaker.ExecuteAsync(async () => { return await planetService.AddPlanetAsync(planet); }); 

        [HttpPost]
        public async Task<IEnumerable<Planet>> Post([FromBody]IEnumerable<Planet> planets) => await circuitBreaker.ExecuteAsync(async () => { return await planetService.AddPlanetsAsync(planets); }); 

        [HttpPut("{id:int}")]
        public async Task<Planet> Put(int id, [FromBody]Planet planet) => await circuitBreaker.ExecuteAsync(async () => { return await planetService.SavePlanetAsync(planet); }); 

        [HttpDelete("{id:int}")]
        public async Task Delete(int id) => await circuitBreaker.ExecuteAsync(async () => { await planetService.DeletePlanetAsync(id); }); 
    }
}