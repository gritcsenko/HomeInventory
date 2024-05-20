namespace HomeInventory.Domain.Events;

public record DomainEvent(Ulid Id, DateTimeOffset CreatedOn) : IDomainEvent
{
    public DomainEvent(TimeProvider dateTimeService)
        : this(Ulid.NewUlid(), dateTimeService.GetUtcNow())
    {
    }
}
