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
        internal readonly DBContext _dataContext;
        private readonly DbSet<T> _dbSet;

        public Repository(DBContext dataContext)
        {
            this._dataContext = dataContext;
            _dbSet = dataContext.Set<T>();
        }

        public async Task<IQueryable<T>> GetQueryableAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> where)
        {
            return await _dbSet.Where(where).ToListAsync();
        }

        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            return await _dbSet.SingleOrDefaultAsync(where);
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> where)
        {
            return await _dbSet.FirstOrDefaultAsync(where);
        }

        public async Task<T> AddAsync(T entity)
        {
            if (entity.Id == default(Guid))
            {
                throw new ArgumentException();
            }

            _dbSet.Add(entity);
            await SaveChangesAsync();
            return await SingleOrDefaultAsync(x => x.Id == entity.Id);
        }

        public async Task<List<T>> AddRangeAsync(List<T> entities)
        {
            if (entities.Any(e => e.Id == default(Guid)))
            {
                throw new ArgumentException();
            }

            _dbSet.AddRange(entities);
            await SaveChangesAsync();
            var savedEntities = new List<T>();

            foreach (var entity in entities)
            {
                savedEntities.Add(await SingleOrDefaultAsync(x => x.Id == entity.Id));
            }
            return savedEntities;
        }

        public async Task<T> SaveAsync(T entity)
        {
            if (entity.Id == default(Guid))
            {
                throw new ArgumentException();
            }
            _dataContext.Entry(entity).State = EntityState.Modified;
            await SaveChangesAsync();
            return await SingleOrDefaultAsync(x => x.Id == entity.Id);
        }

        public async Task DeleteAsync(T entity)
        {
            if (entity.Id == default(Guid))
            {
                throw new ArgumentException();
            }
            _dataContext.Entry(entity).State = EntityState.Deleted;
            await SaveChangesAsync();
        }

        internal async Task SaveChangesAsync()
        {
            try
            {
                await _dataContext.SaveChangesAsync();
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
                _dataContext.Dispose();
            }
            disposed = true;
        }
        #endregion
    }
}
