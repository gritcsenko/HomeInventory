using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.Events;

public record DomainEvent(Ulid Id, DateTimeOffset CreatedOn) : IDomainEvent
{
    public DomainEvent(IIdSupplier<Ulid> supplier, TimeProvider dateTimeService)
        : this(supplier.Supply(), dateTimeService.GetUtcNow())
    {
    }
}
