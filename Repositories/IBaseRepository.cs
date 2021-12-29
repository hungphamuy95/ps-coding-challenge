using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IBaseRepository<TEntity> where  TEntity:class
    {
        Task<TEntity> GetAsync(object id);
        IQueryable<TEntity> GetAsQueryable();
        void Delete(TEntity entity);
        void Update(TEntity entity, IEnumerable<string> excludeFieldNames = null);
        void Add(TEntity entity);
        Task SaveChangesAsync();
        Task<bool> Exist(Expression<Func<TEntity, bool>> spec = null);
        Task<int> Count(Expression<Func<TEntity, bool>> spec = null);
    }
}
