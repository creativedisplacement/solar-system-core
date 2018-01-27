using Microsoft.AspNetCore.Mvc;
using SolarSystemCore.Models;
using SolarSystemCore.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolarSystemCore.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Moon")]
    public class MoonController : Controller
    {
        private readonly IMoonService moonService;

        public MoonController(IMoonService moonService) => this.moonService = moonService;

        [HttpGet]
        public async Task<IEnumerable<Moon>> Get() => await moonService.GetAllMoonsAsync();

        [HttpGet("{id:int}")]
        public async Task<Moon> Get(int id) => await moonService.GetMoonAsync(id);

        [HttpGet("planet/{id:int}")]
        public async Task<IEnumerable<Moon>> Get(int id, string star) => await moonService.GetAllMoonsByPlanetIdAsync(id);

        [HttpPost]
        public async Task<Moon> Post([FromBody]Moon moon) => await moonService.AddMoonAsync(moon);

        [HttpPost]
        public async Task<IEnumerable<Moon>> Post([FromBody]IEnumerable<Moon> moons) => await moonService.AddMoonsAsync(moons);

        [HttpPut("{id:int}")]
        public async Task<Moon> Put(int id, [FromBody]Moon moon) => await moonService.SaveMoonAsync(moon);

        [HttpDelete("{id}")]
        public async Task Delete(int id) => await moonService.DeleteMoonAsync(id);
    }
}