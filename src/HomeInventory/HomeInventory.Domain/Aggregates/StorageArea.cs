using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Events;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Aggregates;

public class StorageArea(StorageAreaId id) : AggregateRoot<StorageArea, StorageAreaId>(id)
{
    private readonly LinkedList<Product> _products = new();

    public IReadOnlyCollection<Product> Products => _products;

    public required StorageAreaName Name { get; init; }

    public Option<DuplicateProductError> Add(IIdSupplier<Ulid> supplier, Product item, TimeProvider dateTimeService)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(dateTimeService);

        if (_products.Contains(item))
        {
            return new DuplicateProductError(item);
        }

        _products.AddLast(item);
        AddDomainEvent(new ProductAddedEvent(supplier, dateTimeService, this, item));
        return OptionNone.Default;
    }

    public Option<NotFoundError> Remove(IIdSupplier<Ulid> supplier, Product item, TimeProvider dateTimeService)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(dateTimeService);

        if (!_products.Contains(item))
        {
            return new NotFoundError($"Product with Id {item.Id} not found");
        }

        _products.Remove(item);
        AddDomainEvent(new ProductRemovedEvent(supplier, dateTimeService, this, item));
        return OptionNone.Default;
    }
}