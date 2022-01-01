using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IBaseRepository<TEntity> where  TEntity:class
    {
        /// <summary>
        /// Asynchronous method that get single record from database by its id 
        /// </summary>
        /// <param name="id">id or unique key of record</param>
        /// <returns>
        /// Single record is specified by its id
        /// </returns>
        Task<TEntity> GetAsync(object id);

        /// <summary>
        /// Populate IQueryable for any entity 
        /// </summary>
        /// <returns>
        /// IQueryable for any entity
        /// </returns>
        IQueryable<TEntity> GetAsQueryable();

        /// <summary>
        /// Remove any record of an entity from database
        /// </summary>
        /// <param name="entity">Entity Object is needed to delete</param>
        /// <returns>
        /// Single record is specified by its id
        /// </returns>
        void Delete(TEntity entity);

        /// <summary>
        /// Update any property of an entity
        /// </summary>
        /// <param name="entity">Entity Object is needed to update</param>
        /// <param name="excludeFieldNames">Collection of fieldnames that should be excluded in update state. Default value is null</param>
        void Update(TEntity entity, IEnumerable<string> excludeFieldNames = null);

        /// <summary>
        /// Add new record of entity to database
        /// </summary>
        /// <param name="entity">Entity Object is needed to add</param>
        void Add(TEntity entity);

        /// <summary>
        /// Async method that save all changes to database
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Check record is existed by any field name
        /// </summary>
        /// <param name="spec">Lambada function</param>
        /// <returns>
        /// True: Record is existed, False: Not found any record
        /// </returns>
        Task<bool> Exist(Expression<Func<TEntity, bool>> spec = null);

        /// <summary>
        /// Count total record of any entity
        /// </summary>
        /// <param name="spec">Lambada function</param>
        /// <returns>
        /// The total number of record
        /// </returns>
        Task<int> Count(Expression<Func<TEntity, bool>> spec = null);
    }
}
