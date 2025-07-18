using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Utilities.Generics
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext _context;
        protected DbSet<TEntity> _dbSet;

        protected Repository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? expression = null)
        {
            return expression != null ? await _dbSet.AnyAsync(expression) : await _dbSet.AnyAsync();
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? expression = null)
        {
            return expression != null ? await _dbSet.CountAsync(expression) : await _dbSet.CountAsync();
        }

        public virtual async Task CreateManyAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual async Task CreateOneAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task DeleteManyAsync(Expression<Func<TEntity, bool>>? expression = null)
        {
            var entities = await FindManyAsync(expression);
            await DeleteManyAsync(entities);
        }

        public virtual async Task DeleteManyAsync(IEnumerable<TEntity> entities)
        {
            await Task.Run(() => _dbSet.RemoveRange(entities));
        }

        public virtual async Task DeleteOneAsync(TEntity entity)
        {
            await Task.Run(() => _dbSet.Remove(entity));
        }

        public virtual async Task DeleteOneAsync(object entityKey)
        {
            var entity = await FindOneByKeyAsync(entityKey);
            if (entity != null)
            {
                await DeleteOneAsync(entity);
            }
        }

        public async Task<bool> ExistsAsync(object entityKey)
        {
            return await FindOneByKeyAsync(entityKey) != null;
        }

        public virtual async Task<TEntity?> FindFirstAsync(Expression<Func<TEntity, bool>>? expression = null)
        {
            return expression != null ? await _dbSet.FirstOrDefaultAsync(expression) : await _dbSet.FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> FindManyAsync(Expression<Func<TEntity, bool>>? expression = null, params string[] includes)
        {
            IQueryable<TEntity> entities = expression != null ? _dbSet.Where(expression) : _dbSet;

            foreach (var include in includes)
            {
                entities = entities.Include(include);
            }

            return await entities.ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindManyPagedAsync(int page, int pageSize, Expression<Func<TEntity, bool>>? expression = null)
        {
            if (expression != null)
            {
                return await _dbSet.Where(expression)
                                 .Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
            }
            else
            {
                return await _dbSet.Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
            }
        }

        public virtual async Task<IEnumerable<TEntity>> FindManyWithOrderedAsync<TKey>(Expression<Func<TEntity, TKey>> orderBy, bool ascending = true, Expression<Func<TEntity, bool>>? expression = null)
        {
            IQueryable<TEntity> entities = expression != null ? _dbSet.Where(expression) : _dbSet;
            if (ascending)
            {
                return await entities.OrderBy(orderBy).ToListAsync();
            }
            else
            {
                return await entities.OrderByDescending(orderBy).ToListAsync();
            }
        }

        public virtual async Task<TEntity?> FindOneByKeyAsync(object entityKey)
        {
            return await _dbSet.FindAsync(entityKey);
        }

        public virtual async Task UpdateManyAsync(IEnumerable<TEntity> entities)
        {
            await Task.Run(() => _dbSet.UpdateRange(entities));
        }

        public virtual async Task UpdateOneAsync(TEntity entity)
        {
            await Task.Run(() => _dbSet.Update(entity));
        }
    }
}
