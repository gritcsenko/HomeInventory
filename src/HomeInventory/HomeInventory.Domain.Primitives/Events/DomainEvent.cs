namespace HomeInventory.Domain.Primitives.Events;

public record DomainEvent(Cuid Id, DateTimeOffset CreatedOn) : IDomainEvent
{
    public DomainEvent(ISupplier<Cuid> supplier, TimeProvider dateTimeService)
        : this(supplier.Invoke(), dateTimeService.GetUtcNow())
    {
    }
}
