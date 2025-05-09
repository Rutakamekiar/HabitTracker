using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Infrastructure.Repositories;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext Context;
    private readonly DbSet<TEntity> _dbSet;

    protected Repository(ApplicationDbContext context)
    {
        Context = context;
        _dbSet = Context.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, List<Expression<Func<TEntity, object>>>? includes = null)
    {
        var query = _dbSet.Where(predicate);
      
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(string id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public void Delete(string id)
    {
        var entity = _dbSet.Find(id) ?? throw new KeyNotFoundException();
        _dbSet.Remove(entity);
    }

    public IQueryable<TEntity> AsQueryable(Expression<Func<TEntity, bool>> predicate)
    {
        return _dbSet.Where(predicate).AsQueryable();
    }
}