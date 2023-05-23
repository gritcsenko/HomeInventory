using DotNext;

namespace HomeInventory.Domain.Primitives;

public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>
    where TEntity : class, IEntity<TEntity>
{
    Optional<IUnitOfWork> UnitOfWork { get; }

    /// <summary>
    /// Adds an entity in the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <typeparamref name="TEntity" />.
    /// </returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds the given entities in the database
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <typeparamref name="IEnumerable<T>" />.
    /// </returns>
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an entity in the database
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the given entities in the database
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entity in the database
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the given entities in the database
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task<IUnitOfWork> WithUnitOfWorkAsync(CancellationToken cancellationToken = default);
}
