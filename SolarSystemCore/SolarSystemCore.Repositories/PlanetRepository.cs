using Microsoft.EntityFrameworkCore;
using SolarSystemCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Repositories
{
    public class PlanetRepository : IPlanetRepository
    {
        private readonly Repository<Planet> _repository;

        public PlanetRepository(Repository<Planet> repository) => this._repository = repository;

        public Task<List<Planet>> GetAllPlanets() => _repository._dataContext.Planets.Include(p => p.Moons).ToListAsync();

        public Task<List<Planet>> GetPlanetsByStarId(Guid id) => _repository._dataContext.Planets.Include(p => p.Moons).Where(p => p.StarId == id).ToListAsync();

        public Task<Planet> GetPlanet(Guid id) => _repository._dataContext.Planets.Include(p => p.Moons).Where(p => p.Id == id).SingleOrDefaultAsync();

        public Task<List<Planet>> FindPlanets(Expression<Func<Planet, bool>> where) => _repository._dataContext.Planets.Include(p => p.Moons).Where(where).ToListAsync();

        public Task<Planet> AddPlanet(Planet planet) => _repository.AddAsync(planet);

        public Task<List<Planet>> AddPlanets(List<Planet> planets) => _repository.AddRangeAsync(planets);

        public Task<Planet> SavePlanet(Planet planet) => _repository.SaveAsync(planet);

        public Task DeletePlanet(Planet planet) => _repository.DeleteAsync(planet);
    }
}
