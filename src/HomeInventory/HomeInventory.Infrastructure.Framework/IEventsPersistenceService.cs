using HomeInventory.Domain.Primitives;

namespace HomeInventory.Infrastructure.Persistence;

public interface IEventsPersistenceService
{
    ValueTask SaveEventsAsync(IHasDomainEvents entity, CancellationToken cancellationToken = default);
}
