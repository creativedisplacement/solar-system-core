using SolarSystemCore.Models;
using SolarSystemCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Services
{
    public class PlanetService : IPlanetService
    {
        private IRepository<Planet> planetRepository;

        public PlanetService(IRepository<Planet> planetRepository) => this.planetRepository = planetRepository;

        public async Task<IEnumerable<Planet>> GetAllPlanetsAsync() => await planetRepository.GetAllAsync();

        public async Task<Planet> GetPlanetAsync(int id) => await planetRepository.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<Planet>> FindPlanetsAsync(Expression<Func<Planet, bool>> where) => await planetRepository.FindAsync(where);

        public async Task<int> AddPlanetAsync(Planet planet)
        {
            if (!string.IsNullOrEmpty(planet.Name))
            {
                return await planetRepository.AddAsync(planet);
            }
            throw new NullReferenceException();
        }

        public async Task<int> AddPlanetsAsync(IList<Planet> planets) => await planetRepository.AddRangeAsync(planets);

        public async Task<int> SavePlanetAsync(Planet planet)
        {
            if (planet.Id != 0)
            {
                return await planetRepository.SaveAsync(planet);
            }
            throw new ArgumentException();
        }
       
        public async Task<int> DeletePlanetAsync(int id)
        {
            var planet = await GetPlanetAsync(id);

            if (planet != null)
            {
                return await planetRepository.DeleteAsync(planet);
            }
            throw new ArgumentException();
        }  
    }
}
