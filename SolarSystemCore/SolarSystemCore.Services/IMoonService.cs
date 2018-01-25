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

        Task<int> AddMoonAsync(Moon moon);
        Task<int> AddMoonsAsync(IList<Moon> moons);
        Task<int> SaveMoonAsync(Moon moon);
        Task<int> DeleteMoonAsync(int id);
    }
}
