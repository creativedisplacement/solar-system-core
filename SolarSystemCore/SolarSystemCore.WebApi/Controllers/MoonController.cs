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
        private readonly IMoonService _moonService;
        private readonly ILogger<MoonController> _logger;
        private static Helpers.CircuitBreaker.CircuitBreaker _circuitBreaker;

        public MoonController(IMoonService moonService, IAppSettings appSettings, ILogger<MoonController> logger)
        {
            this._moonService = moonService;
            this._logger = logger;
            if (_circuitBreaker == null)
            {
                _circuitBreaker = new Helpers.CircuitBreaker.CircuitBreaker("moon_breaker", appSettings.FailureThreshold, TimeSpan.FromSeconds(appSettings.OpenCircuitTimeout));
            }
        }

        [HttpGet]
        public async Task<List<Moon>> Get() => await _circuitBreaker.ExecuteAsync(async () => await _moonService.GetAllMoons()); 

        [HttpGet("{id:guid}")]
        public async Task<Moon> Get(Guid id) => await _circuitBreaker.ExecuteAsync(async () => await _moonService.GetMoon(id)); 

        [HttpGet("planet/{id:guid}")]
        public async Task<List<Moon>> Get(Guid id, string star) => await _circuitBreaker.ExecuteAsync(async () => await _moonService.GetMoonsByPlanetId(id)); 

        [HttpPost]
        public async Task<Moon> Post([FromBody]Moon moon) => await _circuitBreaker.ExecuteAsync(async () => await _moonService.AddMoon(moon)); 

        [HttpPost]
        public async Task<List<Moon>> Post([FromBody]List<Moon> moons) => await _circuitBreaker.ExecuteAsync(async () => await _moonService.AddMoons(moons)); 

        [HttpPut("{id:guid}")]
        public async Task<Moon> Put(Guid id, [FromBody]Moon moon) => await _circuitBreaker.ExecuteAsync(async () => await _moonService.SaveMoon(moon)); 

        [HttpDelete("{id:guid}")]
        public async Task Delete(Guid id) => await _circuitBreaker.ExecuteAsync(async () => { await _moonService.DeleteMoon(id); }); 
    }
}