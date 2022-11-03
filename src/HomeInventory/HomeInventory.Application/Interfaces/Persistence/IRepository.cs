using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Interfaces.Persistence;

public interface IRepository<TEntity>
    where TEntity : IEntity<TEntity>
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
}
