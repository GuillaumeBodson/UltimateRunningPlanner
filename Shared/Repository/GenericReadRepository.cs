using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Repository.Abstractions;
using System.Linq.Expressions;

namespace Shared.Repository;

public class GenericReadRepository<TEntity, TPrimaryKey> : BaseGenericRepository<TEntity>, IGenericReadRepository<TEntity, TPrimaryKey>
    where TEntity : class, IDbEntity<TPrimaryKey>
    where TPrimaryKey : struct
{

    public GenericReadRepository(DbContext context, ILogger<GenericReadRepository<TEntity, TPrimaryKey>> logger): base(context, logger)
    {

    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(IIncludeSpecification<TEntity> specification)
    {
        _logger.LogInformation("Retrieving all {EntityType}", typeof(TEntity).Name);
        return await ApplySpecification(_dbSet.AsQueryable(), specification).ToListAsync();
    }

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, IIncludeSpecification<TEntity> specification)
    {
        _logger.LogInformation("Finding {EntityType} with condition", typeof(TEntity).Name);
        return await ApplySpecification(_dbSet, specification).Where(predicate).ToListAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TPrimaryKey id, IIncludeSpecification<TEntity> specification)
    {
        _logger.LogInformation("Retrieving {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
        var entity = await ApplySpecification(_dbSet, specification)
            .FirstOrDefaultAsync(x => x.Id.Equals(id));

        if (entity == null)
            _logger.LogWarning("{EntityType} with ID: {Id} not found", typeof(TEntity).Name, id);

        return entity;
    }
}