using System.Linq.Expressions;

namespace HabitTracker.Infrastructure.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();

    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, List<Expression<Func<TEntity, object>>>? includes = null);

    Task<TEntity?> GetByIdAsync(string id);

    Task AddAsync(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);
    void Delete(string id);
    IQueryable<TEntity> AsQueryable(Expression<Func<TEntity, bool>> predicate);
}