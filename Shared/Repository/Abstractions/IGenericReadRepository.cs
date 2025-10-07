using System.Linq.Expressions;

namespace Shared.Repository.Abstractions;

public interface IGenericReadRepository<TEntity, TPrimaryKey>
    where TEntity : class, IDbEntity<TPrimaryKey>
    where TPrimaryKey : struct
{
    Task<IEnumerable<TEntity>> GetAllAsync(IIncludeSpecification<TEntity> specification);
    Task<TEntity?> GetByIdAsync(TPrimaryKey id, IIncludeSpecification<TEntity> specification);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, IIncludeSpecification<TEntity> specification);
}
