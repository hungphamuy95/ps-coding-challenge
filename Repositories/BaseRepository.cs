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
        public void Dispose()
        {
            _playStudioContext.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task<TEntity> GetAsync(object id) => await _dbSet.FindAsync(id);

        public IQueryable<TEntity> GetAsQueryable() => _dbSet;

        public void Delete(TEntity entity) => _dbSet.Remove(entity);

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

        public void Add(TEntity entity) => _dbSet.Add(entity);

        public async Task SaveChangesAsync() => await _playStudioContext.SaveChangesAsync();

        public async Task<bool> Exist(Expression<Func<TEntity, bool>> spec = null) => await(spec == null ? _dbSet.AnyAsync() : _dbSet.AnyAsync(spec));

        public async Task<int> Count(Expression<Func<TEntity, bool>> spec = null) => await(spec == null ? _dbSet.CountAsync() : _dbSet.CountAsync(spec));
    }
}
