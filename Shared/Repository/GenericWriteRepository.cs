using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Repository.Abstractions;

namespace Shared.Repository;

public class GenericWriteRepository<TEntity, TPrimaryKey> : BaseGenericRepository<TEntity>, IGenericWriteRepository<TEntity, TPrimaryKey>
    where TEntity : class, IDbEntity<TPrimaryKey>
    where TPrimaryKey : struct
{
    public GenericWriteRepository(DbContext context, ILogger<GenericWriteRepository<TEntity, TPrimaryKey>> logger)
        : base(context, logger)
    {
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
            _dbSet.Entry(entity).State = EntityState.Modified;
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
}
