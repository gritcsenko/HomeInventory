using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.Events;

public record DomainEvent(Ulid Id, DateTimeOffset Created) : IDomainEvent
{
    public DomainEvent(IDateTimeService dateTimeService)
        : this(Ulid.NewUlid(), dateTimeService.UtcNow)
    {
    }
}
