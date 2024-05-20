using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.Events;

public record ProductAddedEvent : DomainEvent
{
    public ProductAddedEvent(TimeProvider dateTimeService, Product product)
        : base(dateTimeService)
    {
        Product = product;
    }

    public Product Product { get; }
}
