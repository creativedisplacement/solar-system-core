using SolarSystemCore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SolarSystemCore.Core.Entities;

namespace SolarSystemCore.Core.Services
{
    public class PlanetService : IPlanetService
    {
        private readonly IPlanetRepository _planetRepository;

        public PlanetService(IPlanetRepository planetRepository) => this._planetRepository = planetRepository;

        public Task<List<Planet>> GetAllPlanets() => _planetRepository.GetAllPlanets();

        public Task<List<Planet>> GetPlanetsByStarId(Guid starId) => _planetRepository.GetPlanetsByStarId(starId);

        public Task<Planet> GetPlanet(Guid id) => _planetRepository.GetPlanet(id);

        public Task<List<Planet>> FindPlanets(Expression<Func<Planet, bool>> where) => _planetRepository.FindPlanets(where);

        public Task<Planet> AddPlanet(Planet planet)
        {
            if (!string.IsNullOrEmpty(planet.Name))
            {
                return _planetRepository.AddPlanet(planet);
            }
            throw new NullReferenceException();
        }

        public Task<List<Planet>> AddPlanets(List<Planet> planets)
        {
            var planetsToAdd = planets.Where(p => p.Name != null || p.Name != string.Empty).ToList();

            if (planetsToAdd.Any())
            {
                return _planetRepository.AddPlanets(planetsToAdd);
            }
            throw new ArgumentException();
        }
        
        public Task<Planet> SavePlanet(Planet planet)
        {
            if (planet.Id != Guid.Empty)
            {
                return _planetRepository.SavePlanet(planet);
            }
            throw new ArgumentException();
        }
       
        public async Task<Planet> DeletePlanet(Guid id)
        {
            var planet = await GetPlanet(id);

            if (planet?.Id != id)
            {
                throw new ArgumentException();
            }
            await _planetRepository.DeletePlanet(planet);
            return planet;
        }  
    }
}
