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
    [Route("api/Star")]
    public class StarController : Controller
    {
        private readonly IStarService _starService;
        private readonly ILogger<StarController> _logger;

        private static Helpers.CircuitBreaker.CircuitBreaker _circuitBreaker;
       
        public StarController(IStarService starService, IAppSettings appSettings, ILogger<StarController> logger)
        {
            this._starService = starService;
            this._logger = logger;
            if (_circuitBreaker == null)
            {
                _circuitBreaker = new Helpers.CircuitBreaker.CircuitBreaker("star_breaker", appSettings.FailureThreshold, TimeSpan.FromSeconds(appSettings.OpenCircuitTimeout));
            }
        }

        [HttpGet]
        public async Task<IEnumerable<Star>> Get() => await _circuitBreaker.ExecuteAsync(async () => await _starService.GetAllStars()); 

        [HttpGet("{id:guid}")]
        public async Task<Star> Get(Guid id) => await _circuitBreaker.ExecuteAsync(async () => await _starService.GetStar(id)); 

        [HttpPost]
        public async Task<Star> Post([FromBody]Star star) => await _circuitBreaker.ExecuteAsync(async () => await _starService.AddStar(star)); 

        [HttpPost]
        public async Task<List<Star>> Post([FromBody]List<Star> stars) => await _circuitBreaker.ExecuteAsync(async () => await _starService.AddStars(stars)); 

        [HttpPut("{id:guid}")]
        public async Task<Star> Put(Guid id, [FromBody]Star star) => await _circuitBreaker.ExecuteAsync(async () => await _starService.SaveStar(star)); 

        [HttpDelete("{id:guid}")]
        public async Task Delete(Guid id) => await _circuitBreaker.ExecuteAsync(async () => { await _starService.DeleteStar(id); }); 
    }
}