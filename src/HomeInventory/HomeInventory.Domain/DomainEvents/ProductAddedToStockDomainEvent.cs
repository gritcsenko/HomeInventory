using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.DomainEvents;

public sealed record class ProductAddedToStockDomainEvent(ProductId ProductId, StockId StockId) : IDomainEvent
{
    public DateTimeOffset TimeStamp { get; init; }
}
