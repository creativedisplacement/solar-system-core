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
        private readonly IStarService starService;
        private readonly IAppSettings appSettings;
        private readonly ILogger<StarController> logger;

        private static Helpers.CircuitBreaker.CircuitBreaker circuitBreaker;
       
        public StarController(IStarService starService, IAppSettings appSettings, ILogger<StarController> logger)
        {
            var x = Guid.NewGuid();
            var y = Guid.Parse("DF9AA280-C912-4E42-A5B5-4573CF97FDB0");

            this.starService = starService;
            this.appSettings = appSettings;
            this.logger = logger;
            if (circuitBreaker == null)
            {
                circuitBreaker = new Helpers.CircuitBreaker.CircuitBreaker("star_breaker", appSettings.FailureThreshold, TimeSpan.FromSeconds(appSettings.OpenCircuitTimeout));
            }
        }

        [HttpGet]
        public async Task<IEnumerable<Star>> Get() => await circuitBreaker.ExecuteAsync(async () => { return await starService.GetAllStarsAsync(); }); 

        [HttpGet("{id:guid}")]
        public async Task<Star> Get(Guid id) => await circuitBreaker.ExecuteAsync(async () => { return await starService.GetStarAsync(id); }); 

        [HttpPost]
        public async Task<Star> Post([FromBody]Star star) => await circuitBreaker.ExecuteAsync(async () => { return await starService.AddStarAsync(star); }); 

        [HttpPost]
        public async Task<IEnumerable<Star>> Post([FromBody]IEnumerable<Star> stars) => await circuitBreaker.ExecuteAsync(async () => { return await starService.AddStarsAsync(stars); }); 

        [HttpPut("{id:guid}")]
        public async Task<Star> Put(Guid id, [FromBody]Star star) => await circuitBreaker.ExecuteAsync(async () => { return await starService.SaveStarAsync(star); }); 

        [HttpDelete("{id:guid}")]
        public async Task Delete(Guid id) => await circuitBreaker.ExecuteAsync(async () => { await starService.DeleteStarAsync(id); }); 
    }
}