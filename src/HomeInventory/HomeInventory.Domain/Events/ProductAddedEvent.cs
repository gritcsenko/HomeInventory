using DotNext;
using HomeInventory.Domain.Entities;

namespace HomeInventory.Domain.Events;

public record ProductAddedEvent : DomainEvent
{
    public ProductAddedEvent(ISupplier<Ulid> supplier, TimeProvider dateTimeService, Product product)
        : base(supplier, dateTimeService)
    {
        Product = product;
    }

    public Product Product { get; }
}
