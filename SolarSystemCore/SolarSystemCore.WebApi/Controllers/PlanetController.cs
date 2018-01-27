using Microsoft.AspNetCore.Mvc;
using SolarSystemCore.Models;
using SolarSystemCore.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolarSystemCore.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class PlanetController : Controller
    {
        private readonly IPlanetService planetService;

        public PlanetController(IPlanetService planetService) => this.planetService = planetService;

        [HttpGet]
        public async Task<IEnumerable<Planet>> Get() => await planetService.GetAllPlanetsAsync();

        [HttpGet("{id:int}")]
        public async Task<Planet> Get(int id) => await planetService.GetPlanetAsync(id);

        [HttpGet("star/{id:int}")]
        public async Task<IEnumerable<Planet>> Get(int id, string star) => await planetService.GetAllPlanetsByStarIdAsync(id);

        [HttpPost]
        public async Task<Planet> Post([FromBody]Planet planet) => await planetService.AddPlanetAsync(planet);

        [HttpPost]
        public async Task<IEnumerable<Planet>> Post([FromBody]IEnumerable<Planet> planets) => await planetService.AddPlanetsAsync(planets);

        [HttpPut("{id:int}")]
        public async Task<Planet> Put(int id, [FromBody]Planet planet) => await planetService.SavePlanetAsync(planet);

        [HttpDelete("{id}")]
        public async Task Delete(int id) => await planetService.DeletePlanetAsync(id);
    }
}