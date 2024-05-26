using DotNext;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Events;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using OneOf;
using OneOf.Types;
using Visus.Cuid;

namespace HomeInventory.Domain.Aggregates;

public class StorageArea(StorageAreaId id) : AggregateRoot<StorageArea, StorageAreaId>(id)
{
    private readonly LinkedList<Product> _products = new();

    public IReadOnlyCollection<Product> Products => _products;

    public required StorageAreaName Name { get; init; }

    public OneOf<Success, DuplicateProductError> Add(ISupplier<Cuid> supplier, Product item, TimeProvider dateTimeService)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(dateTimeService);

        if (_products.Contains(item))
        {
            return new DuplicateProductError(item);
        }

        _products.AddLast(item);
        AddDomainEvent(new ProductAddedEvent(supplier, dateTimeService, this, item));
        return new Success();
    }

    public OneOf<Success, NotFound> Remove(ISupplier<Cuid> supplier, Product item, TimeProvider dateTimeService)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(dateTimeService);

        if (!_products.Contains(item))
        {
            return new NotFound();
        }

        _products.Remove(item);
        AddDomainEvent(new ProductRemovedEvent(supplier, dateTimeService, this, item));
        return new Success();
    }
}