using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Common.IRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity);
        T Add(T entity);
        Task<T> AddWithReturnAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> SearchByAsync(Expression<Func<T, bool>> searchBy, params Expression<Func<T, object>>[] includes);

        Task<T> FindByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);

        Task<T> FindByAsync(Expression<Func<T, bool>> predicate);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
       
        Task UpdateAsync<Tkey>(T entity, Func<T, Tkey> getId);

        Task UpdateRangeAsync(List<T> entities);

        Task DeleteAsync(Expression<Func<T, bool>> identity, params Expression<Func<T, object>>[] includes);

        Task DeleteAsync(T entity);
    }
}
