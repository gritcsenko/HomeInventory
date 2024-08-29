using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Infrastructure.Persistence;

public interface IEventsPersistenceService
{
    Task SaveEventsAsync(IHasDomainEvents entity, CancellationToken cancellationToken = default);
}
