using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositories.DataContext;

namespace Repositories
{
    public class BaseRepository<TEntity> : IDisposable, IBaseRepository<TEntity> where  TEntity :class
    {
        protected readonly DbSet<TEntity> _dbSet;
        private readonly PlayStudioContext _playStudioContext;
        public BaseRepository(PlayStudioContext playStudioContext)
        {
            _playStudioContext = playStudioContext;
            _dbSet = _playStudioContext.Set<TEntity>();
        }

        //Clear all resource to anti-leaked memory
        public void Dispose()
        {
            _playStudioContext.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Asynchronous method that get single record from database by its id 
        /// </summary>
        /// <param name="id">id or unique key of record</param>
        /// <returns>
        /// Single record is specified by its id
        /// </returns>
        public async Task<TEntity> GetAsync(object id) => await _dbSet.FindAsync(id);

        /// <summary>
        /// Populate IQueryable for any entity 
        /// </summary>
        /// <returns>
        /// IQueryable for any entity
        /// </returns>
        public IQueryable<TEntity> GetAsQueryable() => _dbSet;

        /// <summary>
        /// Remove any record of an entity from database
        /// </summary>
        /// <param name="entity">Entity Object is needed to delete</param>
        /// <returns>
        /// Single record is specified by its id
        /// </returns>
        public void Delete(TEntity entity) => _dbSet.Remove(entity);

        /// <summary>
        /// Update any property of an entity
        /// </summary>
        /// <param name="entity">Entity Object is needed to update</param>
        /// <param name="excludeFieldNames">Collection of field names that should be excluded in update state. Default value is null</param>
        public void Update(TEntity entity, IEnumerable<string> excludeFieldNames = null)
        {
            _playStudioContext.Entry(entity).State = EntityState.Modified;
            if (excludeFieldNames?.Any() == true)
            {
                foreach (var fieldName in excludeFieldNames)
                {
                    _playStudioContext.Entry(entity).Property(fieldName).IsModified = false;
                }
            }

            _dbSet.Update(entity);
        }

        /// <summary>
        /// Add new record of entity to database
        /// </summary>
        /// <param name="entity">Entity Object is needed to add</param>
        public void Add(TEntity entity) => _dbSet.Add(entity);

        /// <summary>
        /// Async method that save all changes to database
        /// </summary>
        public async Task SaveChangesAsync() => await _playStudioContext.SaveChangesAsync();

        /// <summary>
        /// Check record is existed by any field name
        /// </summary>
        /// <param name="spec">Lambada function</param>
        /// <returns>
        /// True: Record is existed, False: Not found any record
        /// </returns>
        public async Task<bool> Exist(Expression<Func<TEntity, bool>> spec = null) => await(spec == null ? _dbSet.AnyAsync() : _dbSet.AnyAsync(spec));

        /// <summary>
        /// Count total record of any entity
        /// </summary>
        /// <param name="spec">Lambada function</param>
        /// <returns>
        /// The total number of record
        /// </returns>
        public async Task<int> Count(Expression<Func<TEntity, bool>> spec = null) => await(spec == null ? _dbSet.CountAsync() : _dbSet.CountAsync(spec));
    }
}
