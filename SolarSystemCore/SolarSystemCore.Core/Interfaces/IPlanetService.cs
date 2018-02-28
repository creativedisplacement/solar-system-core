using SolarSystemCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Core.Interfaces
{
    public interface IPlanetService
    {
        Task<List<Planet>> GetAllPlanets();
        Task<List<Planet>> GetPlanetsByStarId(Guid starId);
        Task<Planet> GetPlanet(Guid id);
        Task<List<Planet>> FindPlanets(Expression<Func<Planet, bool>> where);

        Task<Planet> AddPlanet(Planet planet);
        Task<List<Planet>> AddPlanets(List<Planet> planets);
        Task<Planet> SavePlanet(Planet planet);
        Task<Planet> DeletePlanet(Guid id);
    }
}