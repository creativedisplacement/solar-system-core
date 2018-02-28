using SolarSystemCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Core.Interfaces
{
    public interface IPlanetRepository
    {
        Task<List<Planet>> GetAllPlanets();

        Task<List<Planet>> GetPlanetsByStarId(Guid id);

        Task<Planet> GetPlanet(Guid id);

        Task<List<Planet>> FindPlanets(Expression<Func<Planet, bool>> where);

        Task<Planet> AddPlanet(Planet planet);

        Task<List<Planet>> AddPlanets(List<Planet> planets);

        Task<Planet> SavePlanet(Planet planet);

        Task DeletePlanet(Planet planet);
    }
}
