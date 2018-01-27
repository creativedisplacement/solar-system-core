using SolarSystemCore.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Services
{
    public interface IMoonService
    {
        Task<IEnumerable<Moon>> GetAllMoonsAsync();
        Task<IEnumerable<Moon>> GetAllMoonsByPlanetIdAsync(int planetId);
        Task<Moon> GetMoonAsync(int id);
        Task<IEnumerable<Moon>> FindMoonsAsync(Expression<Func<Moon, bool>> where);

        Task<Moon> AddMoonAsync(Moon moon);
        Task<IEnumerable<Moon>> AddMoonsAsync(IEnumerable<Moon> moons);
        Task<Moon> SaveMoonAsync(Moon moon);
        Task DeleteMoonAsync(int id);
    }
}
