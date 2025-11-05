namespace Shared.Repository.Abstractions;

public interface IGenericWriteRepository<TEntity, TPrimaryKey>
    where TEntity : class, IDbEntity<TPrimaryKey>
    where TPrimaryKey : struct
{
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TPrimaryKey id);
}