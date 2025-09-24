using CRM_Homestay.Contract.Bases;
using System.Linq.Expressions;

namespace CRM_Homestay.Database.RepGenerationPatten;

public interface IGenericRepository<T> where T : class
{
    Task<List<T>> GetListAsync(Expression<Func<T, bool>> expression, bool istracked = false);
    List<T> GetList(Expression<Func<T, bool>> expression, bool istracked = false);

    Task<T> GetAsync(Expression<Func<T, bool>> expression);

    Task<List<T>> ToListAsync(bool istracked = false);
    List<T> ToList(bool istracked = false);

    void Update(T entity);
    void UpdateRange(IEnumerable<T> entities);
    void Add(T entity);
    Task AddAsync(T entity);
    void AddRange(IEnumerable<T> entities);

    Task AddRangeAsync(IEnumerable<T> entities);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
    Task<PageResult<T>> GetPageWithFilterAsync(PagingRequest<T> request);
}