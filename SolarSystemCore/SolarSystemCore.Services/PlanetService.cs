using SolarSystemCore.Models;
using SolarSystemCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Services
{
    public class PlanetService : IPlanetService
    {
        private IRepository<Planet> planetRepository;

        public PlanetService(IRepository<Planet> planetRepository) => this.planetRepository = planetRepository;

        public async Task<IEnumerable<Planet>> GetAllPlanetsAsync() => await planetRepository.GetAllAsync();

        public async Task<IEnumerable<Planet>> GetAllPlanetsByStarIdAsync(Guid starId) => await planetRepository.FindAsync(p => p.StarId == starId);

        public async Task<Planet> GetPlanetAsync(Guid id) => await planetRepository.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<Planet>> FindPlanetsAsync(Expression<Func<Planet, bool>> where) => await planetRepository.FindAsync(where);

        public async Task<Planet> AddPlanetAsync(Planet planet)
        {
            if (!string.IsNullOrEmpty(planet.Name))
            {
                return await planetRepository.AddAsync(planet);
            }
            throw new NullReferenceException();
        }

        public async Task<IEnumerable<Planet>> AddPlanetsAsync(IEnumerable<Planet> planets)
        {
            var planetsToAdd = planets.Where(p => p.Name != null || p.Name != string.Empty).ToList();

            if (planetsToAdd.Count() > 0)
            {
                return await planetRepository.AddRangeAsync(planets);
            }
            throw new ArgumentException();
        }
        
        public async Task<Planet> SavePlanetAsync(Planet planet)
        {
            if (planet.Id != Guid.Empty)
            {
                return await planetRepository.SaveAsync(planet);
            }
            throw new ArgumentException();
        }
       
        public async Task DeletePlanetAsync(Guid id)
        {
            var planet = await GetPlanetAsync(id);

            if (planet != null && planet.Id == id)
            {
                await planetRepository.DeleteAsync(planet);
                return;
            }
            throw new ArgumentException();
        }  
    }
}
