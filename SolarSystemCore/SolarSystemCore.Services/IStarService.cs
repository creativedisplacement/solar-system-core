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

        Task<Star> AddStarAsync(Star star);
        Task<IEnumerable<Star>> AddStarsAsync(IEnumerable<Star> stars);
        Task<Star> SaveStarAsync(Star star);
        Task DeleteStarAsync(int id);
    }
}
