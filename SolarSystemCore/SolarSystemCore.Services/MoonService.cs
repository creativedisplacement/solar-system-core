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
        private IRepository<Moon> moonRepository;

        public MoonService(IRepository<Moon> moonRepository) => this.moonRepository = moonRepository;

        public async Task<IEnumerable<Moon>> GetAllMoonsAsync() => await moonRepository.GetAllAsync();

        public async Task<IEnumerable<Moon>> GetAllMoonsByPlanetIdAsync(int planetId) => await moonRepository.FindAsync(p => p.PlanetId == planetId);

        public async Task<Moon> GetMoonAsync(int id) => await moonRepository.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<Moon>> FindMoonsAsync(Expression<Func<Moon, bool>> where) => await moonRepository.FindAsync(where);

        public async Task<int> AddMoonAsync(Moon moon)
        {
            if (!string.IsNullOrEmpty(moon.Name))
            {
                return await moonRepository.AddAsync(moon);
            }
            throw new NullReferenceException();
        }

        public async Task<int> AddMoonsAsync(IList<Moon> moons)
        {
            var moonsToAdd = moons.Where(p => p.Name != null || p.Name != string.Empty).ToList();

            if (moonsToAdd.Count() > 0)
            {
                return await moonRepository.AddRangeAsync(moons);
            }
            throw new ArgumentException();
        }

        public async Task<int> SaveMoonAsync(Moon moon)
        {
            if (moon.Id != 0)
            {
                return await moonRepository.SaveAsync(moon);
            }
            throw new ArgumentException();
        }

        public async Task<int> DeleteMoonAsync(int id)
        {
            var moon = await GetMoonAsync(id);

            if (moon != null && moon.Id == id)
            {
                return await moonRepository.DeleteAsync(moon);
            }
            throw new ArgumentException();
        }
    }
}
