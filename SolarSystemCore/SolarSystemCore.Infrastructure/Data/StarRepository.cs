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
    public class StarRepository : IStarRepository
    {
        private readonly Repository<Star> _repository;

        public StarRepository(Repository<Star> repository) => this._repository = repository;

        public Task<List<Star>> GetAllStars() => _repository._dataContext.Stars.Include(p => p.Planets).ToListAsync();

        public Task<Star> GetStar(Guid id) => _repository._dataContext.Stars.Include(p => p.Planets).Where(p => p.Id == id).SingleOrDefaultAsync();

        public Task<List<Star>> FindStars(Expression<Func<Star, bool>> where) => _repository._dataContext.Stars.Include(p => p.Planets).Where(where).ToListAsync();

        public Task<Star> AddStar(Star star) => _repository.AddAsync(star);

        public Task<List<Star>> AddStars(List<Star> stars) => _repository.AddRangeAsync(stars);

        public Task<Star> SaveStar(Star star) => _repository.SaveAsync(star);

        public Task DeleteStar(Star star) => _repository.DeleteAsync(star);
    }
}
