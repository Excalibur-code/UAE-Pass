using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using UAE_Pass_Poc.DBContext;
using UAE_Pass_Poc.Entities;
using UAE_Pass_Poc.Models;

namespace UAE_Pass_Poc.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    protected readonly UaePassDbContext dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public Repository(UaePassDbContext context)
    {
        dbContext = context;
        _dbSet = dbContext.Set<TEntity>();
    }

    public Task InsertAsync(TEntity entity)
    {
        dbContext.Add(entity);
        return dbContext.SaveChangesAsync();
    }

    public async Task<TEntity?> FindAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }
    public async Task<int> UpdateAsync(TEntity entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        dbContext.Update(entity);
        return await dbContext.SaveChangesAsync();
    }

    public async Task<int> UpdateAsync(IEnumerable<TEntity> entities)
    {
        foreach (var x in entities)
        {
            x.UpdatedAt = DateTime.UtcNow;
        }
        _dbSet.UpdateRange(entities);
        return await dbContext.SaveChangesAsync();
    }


    public void Add(TEntity entity)
    {
        dbContext.Add(entity);
    }

    public void Update(TEntity entity)
    {
        dbContext.Update(entity);
    }

    public void Remove(TEntity entity)
    {
        dbContext.Remove(entity);
    }

    public Task SaveAsync()
    {
        return dbContext.SaveChangesAsync();
    }

    public IQueryable<TEntity> AsQueryable()
    {
        return dbContext.Set<TEntity>().AsQueryable();
    }

    public IQueryable<TEntity> AsQueryable(Func<TEntity, bool> filter)
    {
        return dbContext.Set<TEntity>().Where(filter).AsQueryable();
    }

    /// <inheritdoc />
    public virtual TEntity? GetFirstOrDefault(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = GetQueryable(predicate, include, disableTracking, ignoreQueryFilters);

        if (orderBy != null)
        {
            return orderBy(query).FirstOrDefault();
        }

        return query.FirstOrDefault();
    }

    public async Task<TEntity?> GetFirstOrDefaultAsync(string id)
    {
        if(!Guid.TryParse(id, out var entityId))
        {
            throw new ArgumentException("Please pass valid identifier");
        }

        return await GetFirstOrDefaultAsync(entityId);
    }

    public async Task<TEntity?> GetFirstOrDefaultAsync(Guid id)
    {

        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<TEntity?> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = GetQueryable(predicate, include, disableTracking, ignoreQueryFilters);

        if (orderBy != null)
        {
            return await orderBy(query).FirstOrDefaultAsync();
        }

        return await query.FirstOrDefaultAsync();
    }

    ///<inheritdoc/>
    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null)
    {
        if (selector == null)
        {
            return await _dbSet.AnyAsync();
        }

        return await _dbSet.AnyAsync(selector);
    }

    public async Task<int> CountAsync(
    Expression<Func<TEntity, bool>>? predicate,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
    bool disableTracking = true,
    bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = GetQueryable(predicate, include, disableTracking, ignoreQueryFilters);

        if (orderBy != null)
        {
            return await orderBy(query).CountAsync();
        }

        return await query.CountAsync();
    }

    ///<inheritdoc/>
    public async Task<IList<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    ///<inheritdoc/>
    public async Task<IList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false)
    {
        IQueryable<TEntity> query = GetQueryable(predicate, include, disableTracking, ignoreQueryFilters);

        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync();
        }

        return await query.ToListAsync();
    }

    public IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false)
            {
                IQueryable<TEntity> query = GetQueryable(predicate, include, disableTracking, ignoreQueryFilters);

                if (orderBy != null)
                {
                    return orderBy(query);
                }

                return query;
    }

    /// <inheritdoc />
    public virtual IPagedList<TEntity> GetPagedList(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool disableTracking = true,
        bool ignoreQueryFilters = false
    )
    {
        IQueryable<TEntity> query = GetQueryable(predicate, include, disableTracking, ignoreQueryFilters);
        if (orderBy != null)
        {
            return orderBy(query).ToPagedList(pageIndex, pageSize);
        }

        return query.ToPagedList(pageIndex, pageSize);
    }

    /// <inheritdoc />
    public virtual Task<IPagedList<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool disableTracking = true,
        bool ignoreQueryFilters = false
    )
    {
        IQueryable<TEntity> query = GetQueryable(predicate, include, disableTracking, ignoreQueryFilters);
        if (orderBy != null)
        {
            return orderBy(query).ToPagedListAsync(pageIndex, pageSize, 0, default);
        }

        return query.ToPagedListAsync(pageIndex, pageSize, 0, default);
    }

    /// <inheritdoc />
    public Task<IPagedList<TEntity>> GetPagedListAsync(
        IQueryable<TEntity> query,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> _query = query;
        if (include != null)
        {
            _query = include(query);
        }
        return _query.ToPagedListAsync(pageIndex, pageSize, cancellationToken: cancellationToken);
    }



    private IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>>? predicate,
      Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include,
      bool disableTracking,
      bool ignoreQueryFilters)
    {
        IQueryable<TEntity> query = _dbSet;

        if (disableTracking)
        {
            query = query.AsNoTracking();
        }

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (ignoreQueryFilters)
        {
            query = query.IgnoreQueryFilters();
        }

        return query;
    }
}