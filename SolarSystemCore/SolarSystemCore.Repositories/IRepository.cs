using SolarSystemCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IQueryable<T>> GetQueryableAsync();
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> where);
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> where);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where);

        Task<int> AddAsync(T entity);
        Task<int> AddRangeAsync(IList<T> entities);
        Task<int> SaveAsync(T entity);
        Task<int> DeleteAsync(T entity);
    }
}
