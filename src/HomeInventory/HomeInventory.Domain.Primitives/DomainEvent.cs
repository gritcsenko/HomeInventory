namespace HomeInventory.Domain.Events;

public record DomainEvent(Cuid Id, DateTimeOffset CreatedOn) : IDomainEvent
{
    public DomainEvent(TimeProvider dateTimeService)
        : this(Cuid.NewCuid(), dateTimeService.GetUtcNow())
    {
    }
}
