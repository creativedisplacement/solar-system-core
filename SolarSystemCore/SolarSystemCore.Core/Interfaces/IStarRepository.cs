using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SolarSystemCore.Core.Entities;

namespace SolarSystemCore.Core.Interfaces
{
    public interface IStarRepository
    {
        Task<List<Star>> GetAllStars();

        Task<Star> GetStar(Guid id);

        Task<List<Star>> FindStars(Expression<Func<Star, bool>> where);

        Task<Star> AddStar(Star star);

        Task<List<Star>> AddStars(List<Star> stars);

        Task<Star> SaveStar(Star star);

        Task DeleteStar(Star star);
    }
}
