namespace HomeInventory.Domain.Primitives;

public interface IReadOnlyRepository<TAggregateRoot>
    where TAggregateRoot : class, IEntity<TAggregateRoot>
{
    /// <summary>
    /// Returns the total number of records.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// number of elements in the input sequence.
    /// </returns>
    ValueTask<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a boolean whether any entity exists or not.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains true if the 
    /// source sequence contains any elements; otherwise, false.
    /// </returns>
    ValueTask<bool> AnyAsync(CancellationToken cancellationToken = default);

    IAsyncEnumerable<TAggregateRoot> GetAllAsync(CancellationToken cancellationToken = default);
}
