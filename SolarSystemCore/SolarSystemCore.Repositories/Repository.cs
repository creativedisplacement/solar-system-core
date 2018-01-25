using Microsoft.EntityFrameworkCore;
using SolarSystemCore.Data;
using SolarSystemCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SolarSystemCore.Repositories
{
    public class Repository<T> : IDisposable, IRepository<T> where T : BaseEntity
    {
        private readonly DBContext dataContext;
        private DbSet<T> DbSet;

        public Repository(DBContext dataContext)
        {
            this.dataContext = dataContext;
            DbSet = dataContext.Set<T>();
        }

        public async Task<IQueryable<T>> GetQueryableAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> where)
        {
            return await DbSet.Where(where).ToListAsync();
        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            return await DbSet.SingleOrDefaultAsync(where);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            return await DbSet.FirstOrDefaultAsync(where);
        }

        public Task<int> AddAsync(T entity)
        {
            DbSet.Add(entity);
            return SaveChangesAsync();
        }

        public Task<int> AddRangeAsync(IList<T> entities)
        {
            DbSet.AddRange(entities);
            return SaveChangesAsync();
        }

        public Task<int> SaveAsync(T entity)
        {
            dataContext.Entry(entity).State = EntityState.Modified;
            return SaveChangesAsync();
        }

        public Task<int> DeleteAsync(T entity)
        {
            dataContext.Entry(entity).State = EntityState.Deleted;
            return SaveChangesAsync();
        }

        internal async Task<int> SaveChangesAsync()
        {
            try
            {
                return await dataContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            catch (DbUpdateException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region "disposing methods"

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                dataContext.Dispose();
            }
            disposed = true;
        }
        #endregion
    }
}
