namespace Shared.Repository.Abstractions;

public interface IGenericRepository<TEntity, TPrimaryKey> : IGenericReadRepository<TEntity, TPrimaryKey>, IGenericWriteRepository<TEntity, TPrimaryKey>
    where TEntity : class, IDbEntity<TPrimaryKey>
    where TPrimaryKey : struct
{

}