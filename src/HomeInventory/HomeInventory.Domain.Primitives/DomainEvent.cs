namespace HomeInventory.Domain.Events;

public record DomainEvent(Ulid Id, DateTimeOffset CreatedOn) : IDomainEvent
{
    public DomainEvent(ISupplier<Ulid> supplier, TimeProvider dateTimeService)
        : this(supplier.Invoke(), dateTimeService.GetUtcNow())
    {
    }
}
