﻿namespace HomeInventory.Domain.Primitives;

public interface IRepository<TAggregateRoot> : IReadOnlyRepository<TAggregateRoot>
    where TAggregateRoot : class, IEntity<TAggregateRoot>
{
    /// <summary>
    /// Adds an entity in the database.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <typeparamref name="TAggregateRoot" />.
    /// </returns>
    ValueTask AddAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds the given entities in the database
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the <typeparamref name="IEnumerable<T>" />.
    /// </returns>
    ValueTask AddRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an entity in the database
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the given entities in the database
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UpdateRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entity in the database
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask DeleteAsync(TAggregateRoot entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes the given entities in the database
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask DeleteRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default);
}
