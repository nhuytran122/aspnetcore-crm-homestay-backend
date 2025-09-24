using CRM_Homestay.Contract.Bases;
using CRM_Homestay.Database.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CRM_Homestay.Database.RepGenerationPatten;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly HomestayContext _context;
    private DbSet<T> Entity { get; set; }
    public GenericRepository(HomestayContext context)
    {
        _context = context;
        Entity = context.Set<T>();
    }

    public DbSet<T> GetQueryable()
    {
        return _context.Set<T>();
    }

    public string GetTableName()
    {
        var entityType = _context.Model.FindEntityType(typeof(T));
        return entityType?.GetTableName() ?? throw new Exception("Table not found!");
    }

    public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> expression, bool istracked = false)
    {
        if (istracked)
        {
            return await _context.Set<T>().Where(expression).ToListAsync();
        }
        return await _context.Set<T>().Where(expression).AsNoTracking().ToListAsync();
    }

    public List<T> GetList(Expression<Func<T, bool>> expression, bool istracked = false)
    {
        if (istracked)
        {
            return _context.Set<T>().Where(expression).AsNoTracking().ToList();
        }
        return _context.Set<T>().Where(expression).AsNoTracking().ToList();
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> expression)
    {
        return (await _context.Set<T>().FirstOrDefaultAsync(expression))!;
    }

    public async Task<List<T>> ToListAsync(bool istracked = false)
    {

        if (istracked)
        {
            return await Entity.ToListAsync();
        }

        return await Entity.AsNoTracking().ToListAsync();
    }

    public List<T> ToList(bool istracked = false)
    {
        if (istracked)
        {
            return Entity.ToList();
        }

        return Entity.AsNoTracking().ToList();
    }

    public async Task<long> GetCountAsync()
    {
        return await _context.Set<T>().CountAsync();
    }

    public void Update(T entity)
    {
        _context.Set<T>().Update(entity);
    }



    public void UpdateRange(IEnumerable<T> entities)
    {
        _context.Set<T>().UpdateRange(entities);
    }

    public void Add(T entity)
    {
        _context.Set<T>().Add(entity);
    }


    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _context.Set<T>().AddRange(entities);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _context.Set<T>().AddRangeAsync(entities);
    }



    public void Remove(T entity)
    {
        _context.Set<T>().Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        _context.Set<T>().RemoveRange(entities);
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
    {
        return await _context.Set<T>().AnyAsync(expression);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> expression)
    {
        return await _context.Set<T>().CountAsync(expression);
    }

    public async Task<PageResult<T>> GetPageWithFilterAsync(PagingRequest<T> request)
    {
        var query = _context.Set<T>().AsQueryable();
        Expression<Func<T, bool>>? filterExpression = null;

        if (request.Filters != null && request.Filters.Any())
        {
            foreach (var filterEntry in request.Filters)
            {
                var fieldName = filterEntry.Key;
                var fieldValue = filterEntry.Value.ToLower();

                if (!string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(fieldValue))
                {
                    var parameter = Expression.Parameter(typeof(T), "entity");
                    var property = Expression.Property(parameter, fieldName);
                    var toLowerMethod = typeof(string).GetMethod("ToLower", new Type[] { })!;
                    var toLowerCall = Expression.Call(property, toLowerMethod);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
                    var containsCall = Expression.Call(toLowerCall, containsMethod, Expression.Constant(fieldValue));

                    var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);

                    if (filterExpression == null)
                    {
                        filterExpression = lambda;
                    }
                    else
                    {
                        filterExpression = filterExpression.And(lambda);
                    }
                }
            }
        }
        if (filterExpression != null)
        {
            query = query.Where(filterExpression);
        }
        var totalItems = await query.CountAsync();
        var items = await query.Skip((request.PageIndex - 1) * request.PageSize)
                              .Take(request.PageSize)
                              .ToListAsync();

        return new PageResult<T>
        {
            TotalRecord = totalItems,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize,
            Items = items
        };
    }
    public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
    {
        return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
    }
}