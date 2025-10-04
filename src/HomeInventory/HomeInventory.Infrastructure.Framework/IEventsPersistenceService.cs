using HomeInventory.Domain.Primitives;

namespace HomeInventory.Infrastructure.Framework;

public interface IEventsPersistenceService
{
    ValueTask SaveEventsAsync(IHasDomainEvents entity, CancellationToken cancellationToken = default);
}
