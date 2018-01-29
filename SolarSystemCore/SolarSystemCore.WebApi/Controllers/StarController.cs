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
    [Route("api/Star")]
    public class StarController : Controller
    {
        private readonly IStarService starService;
        private static Helpers.CircuitBreaker.CircuitBreaker circuitBreaker;
        private readonly IAppSettings appSettings;

        public StarController(IStarService starService, IAppSettings appSettings)
        {
            this.starService = starService;
            this.appSettings = appSettings;
            if (circuitBreaker == null)
            {
                circuitBreaker = new Helpers.CircuitBreaker.CircuitBreaker("star_breaker", appSettings.FailureThreshold, TimeSpan.FromSeconds(appSettings.OpenCircuitTimeout));
            }
        }

        [HttpGet]
        public async Task<IEnumerable<Star>> Get() => await circuitBreaker.ExecuteAsync(async () => { return await starService.GetAllStarsAsync(); }); 

        [HttpGet("{id:int}")]
        public async Task<Star> Get(int id) => await circuitBreaker.ExecuteAsync(async () => { return await starService.GetStarAsync(id); }); 

        [HttpPost]
        public async Task<Star> Post([FromBody]Star star) => await circuitBreaker.ExecuteAsync(async () => { return await starService.AddStarAsync(star); }); 

        [HttpPost]
        public async Task<IEnumerable<Star>> Post([FromBody]IEnumerable<Star> stars) => await circuitBreaker.ExecuteAsync(async () => { return await starService.AddStarsAsync(stars); }); 

        [HttpPut("{id:int}")]
        public async Task<Star> Put(int id, [FromBody]Star star) => await circuitBreaker.ExecuteAsync(async () => { return await starService.SaveStarAsync(star); }); 

        [HttpDelete("{id:int}")]
        public async Task Delete(int id) => await circuitBreaker.ExecuteAsync(async () => { await starService.DeleteStarAsync(id); }); 
    }
}