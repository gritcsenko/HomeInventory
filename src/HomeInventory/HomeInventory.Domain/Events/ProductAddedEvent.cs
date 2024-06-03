using DotNext;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Primitives.Messages;
using Visus.Cuid;

namespace HomeInventory.Domain.Events;

public record ProductAddedEvent : DomainEvent
{
    public ProductAddedEvent(ISupplier<Cuid> supplier, TimeProvider dateTimeService, Product product)
        : base(supplier, dateTimeService)
    {
        Product = product;
    }

    public Product Product { get; }
}
