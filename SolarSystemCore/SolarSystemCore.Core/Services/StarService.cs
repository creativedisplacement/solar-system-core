using SolarSystemCore.Core.Entities;
using SolarSystemCore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Core.Services
{
    public class StarService : IStarService
    {
        private readonly IStarRepository _starRepository;

        public StarService(IStarRepository starRepository) => this._starRepository = starRepository;

        public Task<List<Star>> GetAllStars() => _starRepository.GetAllStars();

        public Task<Star> GetStar(Guid id) => _starRepository.GetStar(id);

        public Task<List<Star>> FindStars(Expression<Func<Star, bool>> where) => _starRepository.FindStars(where);

        public Task<Star> AddStar(Star star)
        {
            if (!string.IsNullOrEmpty(star.Name))
            {
                return _starRepository.AddStar(star);
            }
            throw new NullReferenceException();
        }

        public Task<List<Star>> AddStars(List<Star> stars)
        {
            var starsToAdd = stars.Where(p => p.Name != null || p.Name != string.Empty).ToList();

            if (starsToAdd.Any())
            {
                return _starRepository.AddStars(stars);
            }
            throw new ArgumentException();
        }

        public Task<Star> SaveStar(Star star)
        {
            if (star.Id != Guid.Empty)
            {
                return _starRepository.SaveStar(star);
            }
            throw new ArgumentException();
        }

        public async Task<Star> DeleteStar(Guid id)
        {
            var star = await GetStar(id);

            if (star?.Id != id)
            {
                throw new ArgumentException();
            }
            await _starRepository.DeleteStar(star);
            return star;
        }
    }
}
