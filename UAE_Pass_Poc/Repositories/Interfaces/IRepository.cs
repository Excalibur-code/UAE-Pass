using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using UAE_Pass_Poc.Entities;
using UAE_Pass_Poc.Models;

namespace UAE_Pass_Poc.Repositories;

public interface IRepository<TEntity> where TEntity : Entity
{
    void Add(TEntity entity);
    IQueryable<TEntity> AsQueryable();
    Task InsertAsync(TEntity entity);
    void Remove(TEntity entity);
    Task SaveAsync();
    void Update(TEntity entity);
    Task<TEntity?> FindAsync(object id);
    Task<int> UpdateAsync(TEntity entity);
    Task<int> UpdateAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. 
    /// This method defaults to a read-only, no-tracking query.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="disableTracking"><c>true</c> to disable changing tracking; otherwise, <c>false</c>.
    /// Default to <c>true</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <returns>An <see cref="{TEntity}"/> that contains elements that satisfy 
    /// the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>This method defaults to a read-only, no-tracking query.</remarks>
    TEntity? GetFirstOrDefault(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false
    );

    Task<TEntity?> GetFirstOrDefaultAsync(string id);
    Task<TEntity?> GetFirstOrDefaultAsync(Guid id);

    /// <summary>
    /// Gets the first or default entity based on a predicate, orderby delegate and include delegate. 
    /// This method defaults to a read-only, no-tracking query.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="disableTracking"><c>true</c> to disable changing tracking; otherwise, <c>false</c>.
    /// Default to <c>true</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <returns>An <see cref="{TEntity}"/> that contains elements that satisfy
    /// the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>Ex: This method defaults to a read-only, no-tracking query. </remarks>
    Task<TEntity?> GetFirstOrDefaultAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false
    );

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="orderBy"></param>
    /// <param name="include"></param>
    /// <param name="disableTracking"></param>
    /// <param name="ignoreQueryFilters"></param>
    /// <returns></returns>
    IQueryable<TEntity> GetAll(
        Expression<Func<TEntity, bool>>? predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false
    );



    /// <summary>
    /// Gets the Async Exists record based on a predicate.
    /// </summary>
    /// <param name="selector"></param>
    /// <returns></returns>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? selector = null);


    Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false);

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>        
    /// <returns>An <see cref="IList{TEntity}"/> that contains elements that satisfy 
    /// the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>Ex: This method defaults to a read-only, no-tracking query.</remarks>
    Task<IList<TEntity>> GetAllAsync();

    /// <summary>
    /// Gets all entities. This method is not recommended
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="disableTracking"><c>true</c> to disable changing tracking; 
    /// otherwise, <c>false</c>. Default to <c>true</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <returns>An <see cref="IList{TEntity}"/> that contains elements that satisfy 
    /// the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>Ex: This method defaults to a read-only, no-tracking query.</remarks>
    Task<IList<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true,
        bool ignoreQueryFilters = false
    );

    /// <summary>
    /// Gets the <see cref="IPagedList{TEntity}"/> based on a predicate, orderby delegate and page information. 
    /// This method default no-tracking query.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="pageIndex">The index of page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, 
    /// <c>false</c>. Default to <c>true</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy 
    /// the condition specified by <paramref name="predicate"/>.</returns>
    /// <remarks>This method default no-tracking query.</remarks>
    IPagedList<TEntity> GetPagedList(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool disableTracking = true,
        bool ignoreQueryFilters = false
    );

    /// <summary>
    /// Gets the <see cref="IPagedList{TEntity}"/> based on a predicate, orderby delegate and page information.
    /// This method default no-tracking query.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="orderBy">A function to order elements.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="pageIndex">The index of page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. 
    /// Default to <c>true</c>.</param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition 
    /// specified by <paramref name="predicate"/>.</returns>
    /// <remarks>This method default no-tracking query.</remarks>
    Task<IPagedList<TEntity>> GetPagedListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        bool disableTracking = true,
        bool ignoreQueryFilters = false
    );

    /// <summary>
    /// Gets the <see cref="IPagedList{TEntity}"/> based on a predicate, orderby delegate and page information.
    /// This method default no-tracking query.
    /// </summary>
    /// <param name="query">A query to test each element for a condition.</param>
    /// <param name="include">A function to include navigation properties</param>
    /// <param name="pageIndex">The index of page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="disableTracking"><c>True</c> to disable changing tracking; otherwise, <c>false</c>. 
    /// Default to <c>true</c>.</param>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <param name="ignoreQueryFilters">Ignore query filters</param>
    /// <returns>An <see cref="IPagedList{TEntity}"/> that contains elements that satisfy the condition 
    /// specified by <paramref name="predicate"/>.</returns>
    /// <remarks>This method default no-tracking query.</remarks>
    Task<IPagedList<TEntity>> GetPagedListAsync(
        IQueryable<TEntity> query,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        int pageIndex = 0,
        int pageSize = 20,
        CancellationToken cancellationToken = default
    );
}