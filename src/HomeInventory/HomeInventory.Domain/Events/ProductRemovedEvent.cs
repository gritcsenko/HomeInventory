using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.Events;

public record ProductRemovedEvent : DomainEvent
{
    public ProductRemovedEvent(IDateTimeService dateTimeService, Product product)
        : base(dateTimeService)
    {
        Product = product;
    }

    public Product Product { get; }
}
