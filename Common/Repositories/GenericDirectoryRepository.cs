using Common.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Model.Directory;

namespace Common.Repositories
{
    public class GenericDirectoryRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IDirectoryDbContext _context;

        public GenericDirectoryRepository(IDirectoryDbContext context)
        {
            _context = context;
        }
        public Task AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            return Task.CompletedTask;
        }

        public async Task<T> AddWithReturnAsync(T entity)
        {
            var response = await _context.Set<T>().AddAsync(entity);
            return response.Entity;
        }

        public T Add(T entity)
        {
            var newEntity = _context.Set<T>().Add(entity);
            return newEntity.Entity;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToArrayAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            var result = _context.Set<T>().Where(i => true);

            foreach (var includeExpression in includes)
            {
                result = result.Include(includeExpression);
            }

            return await result.ToArrayAsync();
        }

        public async Task<IEnumerable<T>> SearchByAsync(Expression<Func<T, bool>> searchBy, params Expression<Func<T, object>>[] includes)
        {
            var result = _context.Set<T>().Where(searchBy);

            foreach (var includeExpression in includes)
            {
                result = result.Include(includeExpression);
            }

            return await result.ToArrayAsync();
        }

        public async Task<T> FindByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            var result = _context.Set<T>().AsNoTracking().Where(predicate);

            foreach (var includeExpression in includes)
            {
                result = result.Include(includeExpression);
            }

            return await result.AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<T> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            var result = _context.Set<T>().AsNoTracking().Where(predicate);
            return await result.AsNoTracking().FirstOrDefaultAsync();
        }


        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            var result = _context.Set<T>().AsNoTracking().AnyAsync(predicate);
            return await result;
        }

        public Task UpdateAsync<Tkey>(T entity, Func<T, Tkey> getId)
        {
            var id = getId(entity);
            var local = _context.Set<T>()
                        .Local
                        .FirstOrDefault(entry => getId(entry).Equals(id));

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached;
            }

            _context.Set<T>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public Task UpdateRangeAsync(List<T> entities)
        {
            _context.Set<T>().AttachRange(entities);
            foreach (var entity in entities)
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Expression<Func<T, bool>> identity, params Expression<Func<T, object>>[] includes)
        {
            var results = _context.Set<T>().Where(identity);

            foreach (var includeExpression in includes)
            {
                results = results.Include(includeExpression);
            }

            _context.Set<T>().RemoveRange(results);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }
    }
}
