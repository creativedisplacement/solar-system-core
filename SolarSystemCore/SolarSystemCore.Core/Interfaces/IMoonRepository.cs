using SolarSystemCore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Core.Interfaces
{
    public interface IMoonRepository
    {
        Task<List<Moon>> GetAllMoons();

        Task<List<Moon>> GetMoonsByPlanetId(Guid id);

        Task<Moon> GetMoon(Guid id);

        Task<List<Moon>> FindMoons(Expression<Func<Moon, bool>> where);

        Task<Moon> AddMoon(Moon moon);

        Task<List<Moon>> AddMoons(List<Moon> moons);

        Task<Moon> SaveMoon(Moon moon);

        Task DeleteMoon(Moon moon);
    }
}
