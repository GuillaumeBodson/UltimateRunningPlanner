using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Repository.Abstractions;

namespace Shared.Repository;

public abstract class BaseGenericRepository<TEntity> where TEntity : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly ILogger<BaseGenericRepository<TEntity>> _logger;

    public BaseGenericRepository(DbContext context, ILogger<BaseGenericRepository<TEntity>> logger)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _logger = logger;
    }

    // Helper method to apply specification
    protected IQueryable<TEntity> ApplySpecification(
        IQueryable<TEntity> query,
        IIncludeSpecification<TEntity> specification)
    {
        // Apply Include expressions
        query = specification.Includes
            .Aggregate(query, (current, include) => current.Include(include));

        // Apply string-based includes
        query = specification.IncludeStrings
            .Aggregate(query, (current, include) => current.Include(include));

        return query;
    }
}
