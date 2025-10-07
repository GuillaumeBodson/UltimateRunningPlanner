using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;


namespace Shared.Repository;

public class GenericRepository<TEntity, TPrimaryKey> : IGenericRepository<TEntity, TPrimaryKey> 
    where TEntity : class, IDbEntity<TPrimaryKey>
    where TPrimaryKey : struct
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    protected readonly ILogger<GenericRepository<TEntity, TPrimaryKey>> _logger;

    public GenericRepository(DbContext context, ILogger<GenericRepository<TEntity, TPrimaryKey>> logger)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _logger = logger;
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

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        _logger.LogInformation("Adding new {EntityType}", typeof(TEntity).Name);

        try
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully added {EntityType}", typeof(TEntity).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding {EntityType}", typeof(TEntity).Name);
            throw;
        }

        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        _logger.LogInformation("Updating {EntityType}", typeof(TEntity).Name);

        try
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated {EntityType}", typeof(TEntity).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public virtual async Task DeleteAsync(TPrimaryKey id)
    {
        _logger.LogInformation("Deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);

        var entity = await _dbSet.FindAsync(id);
        if (entity != null)
        {
            try
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully deleted {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
                throw;
            }
        }
        else
        {
            _logger.LogWarning("Attempted to delete non-existent {EntityType} with ID: {Id}", typeof(TEntity).Name, id);
        }
    }

    // Helper method to apply specification
    private static IQueryable<TEntity> ApplySpecification(
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
