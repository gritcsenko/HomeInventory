using DotNext;
using HomeInventory.Domain.Entities;

namespace HomeInventory.Domain.Events;

public record ProductRemovedEvent : DomainEvent
{
    public ProductRemovedEvent(ISupplier<Ulid> supplier, TimeProvider dateTimeService, Product product)
        : base(supplier, dateTimeService)
    {
        Product = product;
    }

    public Product Product { get; }
}
