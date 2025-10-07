using System;
using System.Linq.Expressions;

namespace Shared.Repository;

public interface IGenericRepository<TEntity, TPrimaryKey> 
    where TEntity : class
    where TPrimaryKey : struct
{
    Task<IEnumerable<TEntity>> GetAllAsync(IIncludeSpecification<TEntity> specification);
    Task<TEntity?> GetByIdAsync(TPrimaryKey id, IIncludeSpecification<TEntity> specification);
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, IIncludeSpecification<TEntity> specification);
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TPrimaryKey id);
}