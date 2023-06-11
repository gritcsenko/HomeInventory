using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Entities;

namespace HomeInventory.Domain.Events;

public record ProductRemovedEvent : DomainEvent
{
    public ProductRemovedEvent(DateTimeOffset created, StorageArea area, Product product)
        : base(Guid.NewGuid(), created)
    {
        Area = area;
        Product = product;
    }

    public StorageArea Area { get; }

    public Product Product { get; }
}
