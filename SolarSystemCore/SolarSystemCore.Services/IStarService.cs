using SolarSystemCore.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Services
{
    public interface IStarService
    {
        Task<IEnumerable<Star>> GetAllStarsAsync();
        Task<Star> GetStarAsync(int id);
        Task<IEnumerable<Star>> FindStarsAsync(Expression<Func<Star, bool>> where);

        Task<int> AddStarAsync(Star star);
        Task<int> AddStarsAsync(IList<Star> stars);
        Task<int> SaveStarAsync(Star star);
        Task<int> DeleteStarAsync(int id);
    }
}
