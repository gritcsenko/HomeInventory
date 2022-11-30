using FluentResults;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Events;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Aggregates;

public class StorageArea : AggregateRoot<StorageArea, StorageAreaId>
{
    private readonly LinkedList<Product> _products = new();

    public StorageArea(StorageAreaId id)
        : base(id)
    {
    }

    public IReadOnlyCollection<Product> Products => _products;

    public StorageAreaName Name { get; init; } = null!;

    public Result Add(Product item)
    {
        if (_products.Contains(item))
        {
            return new DuplicateProductError(item);
        }

        _products.AddLast(item);
        AddEvent(new ProductAddedEvent(this, item));
        return Result.Ok();
    }

    public Result Remove(Product item)
    {
        if (!_products.Contains(item))
        {
            return new ProductNotFoundError(item);
        }

        _products.Remove(item);
        AddEvent(new ProductRemovedEvent(this, item));
        return Result.Ok();
    }
}