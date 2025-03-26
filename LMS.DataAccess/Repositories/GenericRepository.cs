using LMS.DataAccess.Data;
using LMS.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LMS.DataAccess.Repositories;
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;
    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    #region R
    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    public async Task<T> GetByIdAsync(string id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public async Task<List<string>> GetDistinctAsync(Expression<Func<T, string>> col)
    {
        return await _dbSet.AsNoTracking().Select(col).Distinct().ToListAsync();
    }

    public async Task<T> FindAsync(Expression<Func<T, bool>>? criteria = null, string[]? includes = null)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();
        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }


        return await query.FirstOrDefaultAsync(criteria);
    }

    public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>>? criteria = null, string[]? includes = null)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (includes != null)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }
        if (criteria != null) query.Where(criteria);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAllAsync(int? skip, int? take, Expression<Func<T, object>>? orderBy = null, bool IsDesc = false, Expression<Func<T, bool>>? criteria = null)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();
        if (criteria != null) query.Where(criteria);

        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);

        if (orderBy != null)
            query = IsDesc ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

        return await query.ToListAsync();
    }
    #endregion

    #region CUD
    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        return entities;
    }

    public Task<T> UpdateAsync(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return Task.FromResult(entity);
    }

    public Task<bool> UpdateRangeAsync(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
        return Task.FromResult(true);
    }

    public Task DeleteAsync(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> DeleteRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        return true;
    }
    #endregion

    #region Aggregate
    public async Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null)
    {
        return criteria == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(criteria);
    }

    public async Task<long> MaxAsync(Expression<Func<T, object>> column)
    {
        return Convert.ToInt64(await _dbSet.MaxAsync(column));
    }

    public async Task<long> MaxAsync(Expression<Func<T, object>> column, Expression<Func<T, bool>>? criteria = null)
    {
        return Convert.ToInt64(await _dbSet.Where(criteria).MaxAsync(column));
    }

    public async Task<bool> IsExistAsync(Expression<Func<T, bool>> criteria)
    {
        return await _dbSet.AnyAsync(criteria);
    }

    public async Task<T> LastAsync(Expression<Func<T, object>> orderBy, Expression<Func<T, bool>>? criteria = null)
    {
        return await _dbSet.OrderByDescending(orderBy).FirstOrDefaultAsync(criteria);
    }
    #endregion
}
