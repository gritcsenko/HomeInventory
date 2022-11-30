namespace HomeInventory.Domain.Primitives;

public interface IRepository<in TEntity>
    where TEntity : IEntity<TEntity>
{
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
}
