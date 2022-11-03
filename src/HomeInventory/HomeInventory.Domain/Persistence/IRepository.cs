using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.Persistence;

public interface IRepository<TEntity>
    where TEntity : IEntity<TEntity>
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
}
