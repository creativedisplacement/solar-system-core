using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SolarSystemCore.Core.Entities;
using SolarSystemCore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolarSystemCore.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PlanetController : Controller
    {
        private readonly IPlanetService _planetService;
        private readonly ILogger<PlanetController> _logger;
        private static Helpers.CircuitBreaker.CircuitBreaker _circuitBreaker;

        public PlanetController(IPlanetService planetService, IAppSettings appSettings, ILogger<PlanetController> logger)
        {
            this._planetService = planetService;
            this._logger = logger;
            if (_circuitBreaker == null)
            {
                _circuitBreaker = new Helpers.CircuitBreaker.CircuitBreaker("planet_breaker", appSettings.FailureThreshold, TimeSpan.FromSeconds(appSettings.OpenCircuitTimeout));
            }
        }

        [HttpGet]
        public async Task<List<Planet>> Get() => await _circuitBreaker.ExecuteAsync(async () => await _planetService.GetAllPlanets());

        [HttpGet("{id:guid}")]
        public async Task<Planet> Get(Guid id) => await _circuitBreaker.ExecuteAsync(async () => await _planetService.GetPlanet(id)); 

        [HttpGet("star/{id:guid}")]
        public async Task<IEnumerable<Planet>> Get(Guid id, string star) => await  _circuitBreaker.ExecuteAsync(async () => await _planetService.GetPlanetsByStarId(id)); 

        [HttpPost]
        public async Task<Planet> Post([FromBody]Planet planet) => await _circuitBreaker.ExecuteAsync(async () => await _planetService.AddPlanet(planet)); 

        [HttpPost]
        public async Task<List<Planet>> Post([FromBody]List<Planet> planets) => await _circuitBreaker.ExecuteAsync(async () => await _planetService.AddPlanets(planets)); 

        [HttpPut("{id:guid}")]
        public async Task<Planet> Put(Guid id, [FromBody]Planet planet) => await _circuitBreaker.ExecuteAsync(async () => await _planetService.SavePlanet(planet)); 

        [HttpDelete("{id:guid}")]
        public async Task Delete(Guid id) => await _circuitBreaker.ExecuteAsync(async () => { await _planetService.DeletePlanet(id); }); 
    }
}