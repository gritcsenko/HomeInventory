namespace HomeInventory.Domain.Events;

public record DomainEvent(Ulid Id, DateTimeOffset CreatedOn) : IDomainEvent
{
    public DomainEvent(IDateTimeService dateTimeService)
        : this(Ulid.NewUlid(), dateTimeService.UtcNow)
    {
    }
}
