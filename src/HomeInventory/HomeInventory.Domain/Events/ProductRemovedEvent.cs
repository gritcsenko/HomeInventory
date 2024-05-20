using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.Events;

public record ProductRemovedEvent : DomainEvent
{
    public ProductRemovedEvent(TimeProvider dateTimeService, StorageArea area, Product product)
        : base(dateTimeService)
    {
        Area = area;
        Product = product;
    }

    public StorageArea Area { get; }

    public Product Product { get; }
}
