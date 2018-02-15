using SolarSystemCore.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Services
{
    public interface IPlanetService
    {
        Task<IEnumerable<Planet>> GetAllPlanetsAsync();
        Task<IEnumerable<Planet>> GetAllPlanetsByStarIdAsync(Guid starId);
        Task<Planet> GetPlanetAsync(Guid id);
        Task<IEnumerable<Planet>> FindPlanetsAsync(Expression<Func<Planet, bool>> where);

        Task<Planet> AddPlanetAsync(Planet planet);
        Task<IEnumerable<Planet>> AddPlanetsAsync(IEnumerable<Planet> planets);
        Task<Planet> SavePlanetAsync(Planet planet);
        Task DeletePlanetAsync(Guid id);
    }
}