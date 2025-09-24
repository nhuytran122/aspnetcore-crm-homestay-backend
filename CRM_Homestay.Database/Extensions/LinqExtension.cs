using System.Linq.Expressions;
using CRM_Homestay.Entity.Bases;
using Microsoft.EntityFrameworkCore;

namespace CRM_Homestay.Database.Helper;

public static class LinqExtension
{
    public static IQueryable<TSource> OrderByIf<TSource, TKey>(this IQueryable<TSource> y,
        bool condition, Expression<Func<TSource, TKey>> keySelector)
    {
        return condition ? y.OrderBy(keySelector) : y;
    }
    public static IQueryable<TSource> OrderByDescIf<TSource, TKey>(this IQueryable<TSource> y,
        bool condition, Expression<Func<TSource, TKey>> keySelector)
    {
        return condition ? y.OrderByDescending(keySelector) : y;
    }


    public static async Task<PagedResult<T>> GetPaged<T>(this IQueryable<T> query,
        int currentPage, int pageSize = 25) where T : class
    {
        var result = new PagedResult<T>();

        result.TotalItems = query.Count();

        var totalPage = (double)result.TotalItems / pageSize;
        result.TotalPage = (int)Math.Ceiling(totalPage);

        var skip = (currentPage - 1) * pageSize;
        result.Items = await query.Skip(skip).Take(pageSize).ToListAsync();

        return result;
    }

    public static PagedResult<T> GetPaged<T>(this IEnumerable<T> query,
        int currentPage, int pageSize = 25) where T : class
    {
        var result = new PagedResult<T>();

        result.TotalItems = query.Count();

        var totalPage = (double)result.TotalItems / pageSize;
        result.TotalPage = (int)Math.Ceiling(totalPage);

        var skip = (currentPage - 1) * pageSize;
        result.Items = query.Skip(skip).Take(pageSize).ToList();

        return result;
    }
}