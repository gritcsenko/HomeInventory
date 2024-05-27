using DotNext;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Primitives.Events;
using Visus.Cuid;

namespace HomeInventory.Domain.Events;

public record ProductRemovedEvent : DomainEvent
{
    public ProductRemovedEvent(ISupplier<Cuid> supplier, TimeProvider dateTimeService, Product product)
        : base(supplier, dateTimeService)
    {
        Product = product;
    }

    public Product Product { get; }
}
