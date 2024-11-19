using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.Events;

public record ProductRemovedEvent : DomainEvent
{
    public ProductRemovedEvent(IIdSupplier<Ulid> supplier, TimeProvider dateTimeService, Product product)
        : base(supplier, dateTimeService) =>
        Product = product;

    public Product Product { get; }
}
