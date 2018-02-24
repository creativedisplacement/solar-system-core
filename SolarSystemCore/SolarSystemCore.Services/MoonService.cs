using SolarSystemCore.Models;
using SolarSystemCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Services
{
    public class MoonService : IMoonService
    {
        private readonly IMoonRepository _moonRepository;

        public MoonService(IMoonRepository moonRepository) => this._moonRepository = moonRepository;

        public Task<List<Moon>> GetAllMoons() => _moonRepository.GetAllMoons();

        public Task<List<Moon>> GetAllMoonsByPlanetId(Guid id) => _moonRepository.GetMoonsByPlanetId(id);

        public Task<Moon> GetMoon(Guid id) => _moonRepository.GetMoon(id);

        public Task<List<Moon>> FindMoons(Expression<Func<Moon, bool>> where) => _moonRepository.FindMoons(where);

        public Task<Moon> AddMoon(Moon moon)
        {
            if (!string.IsNullOrEmpty(moon.Name))
            {
                return _moonRepository.AddMoon(moon);
            }
            throw new NullReferenceException();
        }

        public Task<List<Moon>> AddMoons(List<Moon> moons)
        {
            var moonsToAdd = moons.Where(p => p.Name != null || p.Name != string.Empty).ToList();

            if (moonsToAdd.Any())
            {
                return _moonRepository.AddMoons(moonsToAdd);
            }
            throw new ArgumentException();
        }

        public Task<Moon> SaveMoon(Moon moon)
        {
            if (moon.Id != Guid.Empty)
            {
                return _moonRepository.SaveMoon(moon);
            }
            throw new ArgumentException();
        }

        public async Task<Moon> DeleteMoon(Guid id)
        {
            var moon = await GetMoon(id);

            if (moon?.Id != id)
            {
                throw new ArgumentException();
            }
            await _moonRepository.DeleteMoon(moon);
            return moon;
        }
    }
}
