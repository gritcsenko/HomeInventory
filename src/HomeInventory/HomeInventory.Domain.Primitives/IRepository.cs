namespace HomeInventory.Domain.Primitives;

public interface IRepository<TAggregateRoot> : IReadOnlyRepository<TAggregateRoot>
    where TAggregateRoot : class, IEntity<TAggregateRoot>
{
    /// <summary>
    /// Adds an entity in the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <typeparamref name="TAggregateRoot" />.
    /// </returns>
    Task AddAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds the given entities in the database
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task AddRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an entity in the database
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the given entities in the database
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entity in the database
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the given entities in the database
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default);
}
