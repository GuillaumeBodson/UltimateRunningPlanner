using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Repository.Abstractions;
using System.Linq.Expressions;


namespace Shared.Repository;

public class GenericRepository<TEntity, TPrimaryKey> : BaseGenericRepository<TEntity>, IGenericRepository<TEntity, TPrimaryKey> 
    where TEntity : class, IDbEntity<TPrimaryKey>
    where TPrimaryKey : struct
{
    private readonly IGenericWriteRepository<TEntity, TPrimaryKey> _writeRepository;
    private readonly IGenericReadRepository<TEntity, TPrimaryKey> _readRepository;

    public GenericRepository(DbContext context,
                             ILogger<GenericRepository<TEntity, TPrimaryKey>> logger,
                             IGenericWriteRepository<TEntity, TPrimaryKey> writeRepository,
                             IGenericReadRepository<TEntity, TPrimaryKey> readRepository) : base(context, logger)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(IIncludeSpecification<TEntity> specification)
    {
        return await _readRepository.GetAllAsync(specification);
    }

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, IIncludeSpecification<TEntity> specification)
    {
        return await _readRepository.FindAsync(predicate, specification);
    }

    public virtual async Task<TEntity?> GetByIdAsync(TPrimaryKey id, IIncludeSpecification<TEntity> specification)
    {
        return await _readRepository.GetByIdAsync(id, specification);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity)
    {
        return await _writeRepository.AddAsync(entity);
    }

    public virtual async Task UpdateAsync(TEntity entity)
    {
        await _writeRepository.UpdateAsync(entity);
    }

    public virtual async Task DeleteAsync(TPrimaryKey id)
    {
        await _writeRepository.DeleteAsync(id);
    }
}
