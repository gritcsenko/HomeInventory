using DotNext;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Entities;
using Visus.Cuid;

namespace HomeInventory.Domain.Events;

public record ProductRemovedEvent : DomainEvent
{
    public ProductRemovedEvent(ISupplier<Cuid> supplier, TimeProvider dateTimeService, StorageArea area, Product product)
        : base(supplier, dateTimeService)
    {
        Area = area;
        Product = product;
    }

    public StorageArea Area { get; }

    public Product Product { get; }
}
