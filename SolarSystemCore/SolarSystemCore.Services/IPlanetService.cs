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
        Task<IEnumerable<Planet>> GetAllPlanetsByStarIdAsync(int starId);
        Task<Planet> GetPlanetAsync(int id);
        Task<IEnumerable<Planet>> FindPlanetsAsync(Expression<Func<Planet, bool>> where);

        Task<int> AddPlanetAsync(Planet planet);
        Task<int> AddPlanetsAsync(IList<Planet> planets);
        Task<int> SavePlanetAsync(Planet planet);
        Task<int> DeletePlanetAsync(int id);
    }
}