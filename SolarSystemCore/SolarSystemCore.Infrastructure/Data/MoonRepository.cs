using Microsoft.EntityFrameworkCore;
using SolarSystemCore.Core.Entities;
using SolarSystemCore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Infrastructure.Data
{
    public class MoonRepository : IMoonRepository
    {
        private readonly Repository<Moon> _repository;

        public MoonRepository(Repository<Moon> repository) => this._repository = repository;

        public Task<List<Moon>> GetAllMoons() => _repository._dataContext.Moons.Include(m => m.Planet).ToListAsync();

        public Task<List<Moon>> GetMoonsByPlanetId(Guid id) => _repository._dataContext.Moons.Include(m => m.Planet).Where(m => m.PlanetId == id).ToListAsync();

        public Task<Moon> GetMoon(Guid id) => _repository._dataContext.Moons.Include(m => m.Planet).Where(m => m.Id == id).SingleOrDefaultAsync();

        public Task<List<Moon>> FindMoons(Expression<Func<Moon, bool>> where) => _repository._dataContext.Moons.Include(m => m.Planet).Where(where).ToListAsync();

        public Task<Moon> AddMoon(Moon moon) => _repository.AddAsync(moon);

        public Task<List<Moon>> AddMoons(List<Moon> moons) => _repository.AddRangeAsync(moons);

        public Task<Moon> SaveMoon(Moon moon) => _repository.SaveAsync(moon);

        public Task DeleteMoon(Moon moon) => _repository.DeleteAsync(moon);
    }
}
