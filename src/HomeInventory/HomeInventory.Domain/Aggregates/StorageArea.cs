using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Events;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Domain.Aggregates;

public class StorageArea : AggregateRoot<StorageArea, StorageAreaId>
{
    private readonly LinkedList<Product> _products = new();

    public StorageArea(StorageAreaId id)
        : base(id)
    {
    }

    public IReadOnlyCollection<Product> Products => _products;

    public required StorageAreaName Name { get; init; }

    public OneOf<Success, DuplicateProductError> Add(Product item, IDateTimeService dateTimeService)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (dateTimeService is null)
        {
            throw new ArgumentNullException(nameof(dateTimeService));
        }

        if (_products.Contains(item))
        {
            return new DuplicateProductError(item);
        }

        _products.AddLast(item);
        AddDomainEvent(new ProductAddedEvent(dateTimeService.UtcNow, this, item));
        return new Success();
    }

    public OneOf<Success, NotFound> Remove(Product item, IDateTimeService dateTimeService)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (dateTimeService is null)
        {
            throw new ArgumentNullException(nameof(dateTimeService));
        }

        if (!_products.Contains(item))
        {
            return new NotFound();
        }

        _products.Remove(item);
        AddDomainEvent(new ProductRemovedEvent(dateTimeService.UtcNow, this, item));
        return new Success();
    }
}