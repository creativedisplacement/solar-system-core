using SolarSystemCore.Models;
using SolarSystemCore.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Services
{
    public class StarService : IStarService
    {
        private IRepository<Star> starRepository;

        public StarService(IRepository<Star> starRepository) => this.starRepository = starRepository;

        public async Task<IEnumerable<Star>> GetAllStarsAsync() => await starRepository.GetAllAsync();

        public async Task<Star> GetStarAsync(int id) => await starRepository.FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<Star>> FindStarsAsync(Expression<Func<Star, bool>> where) => await starRepository.FindAsync(where);

        public async Task<Star> AddStarAsync(Star star)
        {
            if (!string.IsNullOrEmpty(star.Name))
            {
                return await starRepository.AddAsync(star);
            }
            throw new NullReferenceException();
        }

        public async Task<IEnumerable<Star>> AddStarsAsync(IEnumerable<Star> stars)
        {
            var starsToAdd = stars.Where(p => p.Name != null || p.Name != string.Empty).ToList();

            if (starsToAdd.Count() > 0)
            {
                return await starRepository.AddRangeAsync(stars);
            }
            throw new ArgumentException();
        }

        public async Task<Star> SaveStarAsync(Star star)
        {
            if (star.Id != 0)
            {
                return await starRepository.SaveAsync(star);
            }
            throw new ArgumentException();
        }

        public async Task DeleteStarAsync(int id)
        {
            var star = await GetStarAsync(id);

            if (star != null && star.Id == id)
            {
                await starRepository.DeleteAsync(star);
                return;
            }
            throw new ArgumentException();
        }
    }
}
