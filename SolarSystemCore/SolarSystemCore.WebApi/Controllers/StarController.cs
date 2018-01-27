using Microsoft.AspNetCore.Mvc;
using SolarSystemCore.Models;
using SolarSystemCore.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolarSystemCore.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Star")]
    public class StarController : Controller
    {
        private readonly IStarService starService;

        public StarController(IStarService starService) => this.starService = starService;

        [HttpGet]
        public async Task<IEnumerable<Star>> Get() => await starService.GetAllStarsAsync();

        [HttpGet("{id:int}")]
        public async Task<Star> Get(int id) => await starService.GetStarAsync(id);

        [HttpPost]
        public async Task<Star> Post([FromBody]Star star) => await starService.AddStarAsync(star);

        [HttpPost]
        public async Task<IEnumerable<Star>> Post([FromBody]IEnumerable<Star> stars) => await starService.AddStarsAsync(stars);

        [HttpPut("{id:int}")]
        public async Task<Star> Put(int id, [FromBody]Star star) => await starService.SaveStarAsync(star);

        [HttpDelete("{id}")]
        public async Task Delete(int id) => await starService.DeleteStarAsync(id);
    }
}