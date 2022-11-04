using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.Persistence;

public interface IRepository<in TEntity>
    where TEntity : IEntity<TEntity>
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
}
