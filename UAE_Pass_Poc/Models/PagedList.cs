using Microsoft.EntityFrameworkCore;

namespace UAE_Pass_Poc.Models;

/// <summary>
/// Provides the interface(s) for paged list of any type.
/// </summary>
/// <typeparam name="T">The type for paging.</typeparam>
public interface IPagedList<T>
{
    /// <summary>
    /// Gets the index start value.
    /// </summary>
    /// <value>The index start value.</value>
    int IndexFrom { get; }
    /// <summary>
    /// Gets the page index (current).
    /// </summary>
    int PageIndex { get; }
    /// <summary>
    /// Gets the page size.
    /// </summary>
    int PageSize { get; }
    /// <summary>
    /// Gets the total count of the list of type <typeparamref name="T"/>
    /// </summary>
    int TotalCount { get; }
    /// <summary>
    /// Gets the total pages.
    /// </summary>
    int TotalPages { get; }
    /// <summary>
    /// Gets the current page items.
    /// </summary>
    IList<T> Items { get; }
    /// <summary>
    /// Gets the has previous page.
    /// </summary>
    /// <value>The has previous page.</value>
    bool HasPreviousPage { get; }

    /// <summary>
    /// Gets the has next page.
    /// </summary>
    /// <value>The has next page.</value>
    bool HasNextPage { get; }
}
public class CustomPagedList<T> : IPagedList<T>
{
    public int IndexFrom { get; } = 0;
    public int PageIndex { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public IList<T> Items { get; }

    public bool HasPreviousPage { get; }
    public bool HasNextPage { get; }

    public CustomPagedList(IEnumerable<T> items,
        int totalCount,
        int pageIndex,
        int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
        HasPreviousPage = PageIndex - IndexFrom > 0;
        HasNextPage = PageIndex - IndexFrom + 1 < TotalPages;
        Items = new List<T>(items);
    }
}
/// <summary>
/// Provides some extension methods for <see cref="IEnumerable{T}"/> to provide paging capability.
/// </summary>
public static class IEnumerablePagedListExtensions
{
    /// <summary>
    /// Converts the specified source to <see cref="IPagedList{T}"/> by 
    /// the specified <paramref name="pageIndex"/> and <paramref name="pageSize"/>.
    /// </summary>
    /// <typeparam name="T">The type of the source.</typeparam>
    /// <param name="source">The source to paging.</param>
    /// <param name="pageIndex">The index of the page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="indexFrom">The start index value.</param>
    /// <returns>An instance of the inherited from <see cref="IPagedList{T}"/> interface.</returns>
    public static IPagedList<T> ToPagedList<T>(
        this IEnumerable<T> source,
        int pageIndex,
        int pageSize,
        int indexFrom = 0
    ) => new PagedList<T>(source, pageIndex, pageSize, indexFrom);

    /// <summary>
    /// Converts the specified source to <see cref="IPagedList{T}"/> by 
    /// the specified <paramref name="converter"/>, <paramref name="pageIndex"/> and <paramref name="pageSize"/>
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="source">The source to convert.</param>
    /// <param name="converter">The converter to change the <typeparamref name="TSource"/> 
    /// to <typeparamref name="TResult"/>.</param>
    /// <param name="pageIndex">The page index.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="indexFrom">The start index value.</param>
    /// <returns>An instance of the inherited from <see cref="IPagedList{T}"/> interface.</returns>
    public static IPagedList<TResult> ToPagedList<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<IEnumerable<TSource>,
        IEnumerable<TResult>> converter,
        int pageIndex,
        int pageSize, int indexFrom = 0
    ) => new PagedList<TSource, TResult>(source, converter, pageIndex, pageSize, indexFrom);
}
public static class IQueryablePageListExtensions
{
    /// <summary>
    /// Converts the specified source to <see cref="IPagedList{T}"/> by the specified 
    /// <paramref name="pageIndex"/> and <paramref name="pageSize"/>.
    /// </summary>
    /// <typeparam name="T">The type of the source.</typeparam>
    /// <param name="source">The source to paging.</param>
    /// <param name="pageIndex">The index of the page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken" /> to observe while waiting for the task to complete.
    /// </param>
    /// <param name="indexFrom">The start index value.</param>
    /// <returns>An instance of the inherited from <see cref="IPagedList{T}"/> interface.</returns>
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> source,
        int pageIndex,
        int pageSize,
        int indexFrom = 0,
        CancellationToken cancellationToken = default)
    {
        ////if (indexFrom > pageIndex)
        ////{
        ////    throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}," +
        ////        $" must indexFrom <= pageIndex");
        ////}

        var count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
        var items = await source.Skip((pageIndex - indexFrom) * pageSize)
                                .Take(pageSize).ToListAsync(cancellationToken).ConfigureAwait(false);

        var pagedList = new PagedList<T>()
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
            IndexFrom = indexFrom,
            TotalCount = count,
            Items = items,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };

        return pagedList;
    }
}/// <summary>
 /// Represents the default implementation of the <see cref="IPagedList{T}"/> interface.
 /// </summary>
 /// <typeparam name="T">The type of the data to page</typeparam>
public class PagedList<T> : IPagedList<T>
{
    /// <summary>
    /// Gets or sets the index of the page.
    /// </summary>
    /// <value>The index of the page.</value>
    public int PageIndex { get; set; }
    /// <summary>
    /// Gets or sets the size of the page.
    /// </summary>
    /// <value>The size of the page.</value>
    public int PageSize { get; set; }
    /// <summary>
    /// Gets or sets the total count.
    /// </summary>
    /// <value>The total count.</value>
    public int TotalCount { get; set; }
    /// <summary>
    /// Gets or sets the total pages.
    /// </summary>
    /// <value>The total pages.</value>
    public int TotalPages { get; set; }
    /// <summary>
    /// Gets or sets the index from.
    /// </summary>
    /// <value>The index from.</value>
    public int IndexFrom { get; set; }

    /// <summary>
    /// Gets or sets the items.
    /// </summary>
    /// <value>The items.</value>
    public IList<T> Items { get; set; }

    /// <summary>
    /// Gets the has previous page.
    /// </summary>
    /// <value>The has previous page.</value>
    public bool HasPreviousPage => PageIndex - IndexFrom > 0;

    /// <summary>
    /// Gets the has next page.
    /// </summary>
    /// <value>The has next page.</value>
    public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{T}" /> class.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="pageIndex">The index of the page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="indexFrom">The index from.</param>
    public PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int indexFrom)
    {
        if (indexFrom > pageIndex)
        {
            throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}," +
                $" must indexFrom <= pageIndex");
        }

        if (source is IQueryable<T> querable)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            IndexFrom = indexFrom;
            TotalCount = querable.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            Items = querable.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();
        }
        else
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            IndexFrom = indexFrom;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            Items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToList();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{T}" /> class.
    /// </summary>
    internal PagedList() => Items = Array.Empty<T>();
}


/// <summary>
/// Provides the implementation of the <see cref="IPagedList{T}"/> and converter.
/// </summary>
/// <typeparam name="TSource">The type of the source.</typeparam>
/// <typeparam name="TResult">The type of the result.</typeparam>
public class PagedList<TSource, TResult> : IPagedList<TResult>
{
    /// <summary>
    /// Gets the index of the page.
    /// </summary>
    /// <value>The index of the page.</value>
    public int PageIndex { get; }
    /// <summary>
    /// Gets the size of the page.
    /// </summary>
    /// <value>The size of the page.</value>
    public int PageSize { get; }
    /// <summary>
    /// Gets the total count.
    /// </summary>
    /// <value>The total count.</value>
    public int TotalCount { get; }
    /// <summary>
    /// Gets the total pages.
    /// </summary>
    /// <value>The total pages.</value>
    public int TotalPages { get; }
    /// <summary>
    /// Gets the index from.
    /// </summary>
    /// <value>The index from.</value>
    public int IndexFrom { get; }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>The items.</value>
    public IList<TResult> Items { get; }

    /// <summary>
    /// Gets the has previous page.
    /// </summary>
    /// <value>The has previous page.</value>
    public bool HasPreviousPage => PageIndex - IndexFrom > 0;

    /// <summary>
    /// Gets the has next page.
    /// </summary>
    /// <value>The has next page.</value>
    public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{TSource, TResult}" /> class.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="converter">The converter.</param>
    /// <param name="pageIndex">The index of the page.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <param name="indexFrom">The index from.</param>
    public PagedList(
        IEnumerable<TSource> source,
        Func<IEnumerable<TSource>,
        IEnumerable<TResult>> converter,
        int pageIndex,
        int pageSize,
        int indexFrom)
    {
        if (indexFrom > pageIndex)
        {
            throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}," +
                $" must indexFrom <= pageIndex");
        }

        if (source is IQueryable<TSource> querable)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            IndexFrom = indexFrom;
            TotalCount = querable.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            var items = querable.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToArray();

            Items = new List<TResult>(converter(items));
        }
        else
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            IndexFrom = indexFrom;
            TotalCount = source.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

            var items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToArray();

            Items = new List<TResult>(converter(items));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedList{TSource, TResult}" /> class.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="converter">The converter.</param>
    public PagedList(IPagedList<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter)
    {
        PageIndex = source.PageIndex;
        PageSize = source.PageSize;
        IndexFrom = source.IndexFrom;
        TotalCount = source.TotalCount;
        TotalPages = source.TotalPages;

        Items = new List<TResult>(converter(source.Items));
    }
}

/// <summary>
/// Provides some help methods for <see cref="IPagedList{T}"/> interface.
/// </summary>
public static class PagedList
{
    /// <summary>
    /// Creates an empty of <see cref="IPagedList{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type for paging </typeparam>
    /// <returns>An empty instance of <see cref="IPagedList{T}"/>.</returns>
    public static IPagedList<T> Empty<T>() => new PagedList<T>();
    /// <summary>
    /// Creates a new instance of <see cref="IPagedList{TResult}"/> 
    /// from source of <see cref="IPagedList{TSource}"/> instance.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="converter">The converter.</param>
    /// <returns>An instance of <see cref="IPagedList{TResult}"/>.</returns>
    public static IPagedList<TResult> From<TResult, TSource>(
        IPagedList<TSource> source,
        Func<IEnumerable<TSource>,
        IEnumerable<TResult>> converter) => new PagedList<TSource, TResult>(source, converter);
}

