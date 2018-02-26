using SolarSystemCore.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Services
{
    public interface IMoonService
    {
        Task<List<Moon>> GetAllMoons();
        Task<List<Moon>> GetMoonsByPlanetId(Guid planetId);
        Task<Moon> GetMoon(Guid id);
        Task<List<Moon>> FindMoons(Expression<Func<Moon, bool>> where);

        Task<Moon> AddMoon(Moon moon);
        Task<List<Moon>> AddMoons(List<Moon> moons);
        Task<Moon> SaveMoon(Moon moon);
        Task<Moon> DeleteMoon(Guid id);
    }
}
