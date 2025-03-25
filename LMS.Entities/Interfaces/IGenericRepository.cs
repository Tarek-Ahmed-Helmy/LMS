using System.Linq.Expressions;

namespace LMS.Entities.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<List<string>> GetDistinctAsync(Expression<Func<T, string>> col);

    Task<T> FindAsync(Expression<Func<T, bool>>? criteria = null, string[]? includes = null);
    Task<IEnumerable<T>> FindAllAsync(int? skip = null, int? take = null,
        Expression<Func<T, object>>? orderBy = null, bool isDesc = false, Expression<Func<T, bool>>? criteria = null);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>>? criteria = null, string[]? includes = null);

    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

    Task<T> UpdateAsync(T entity);
    Task<bool> UpdateRangeAsync(IEnumerable<T> entities);

    Task DeleteAsync(T entity);
    Task<bool> DeleteRangeAsync(IEnumerable<T> entities);

    Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null);

    Task<long> MaxAsync(Expression<Func<T, object>> column);
    Task<long> MaxAsync(Expression<Func<T, object>> column, Expression<Func<T, bool>>? criteria = null);

    Task<bool> IsExistAsync(Expression<Func<T, bool>> criteria);

    Task<T> LastAsync(Expression<Func<T, object>> column, Expression<Func<T, bool>>? criteria = null);
}
