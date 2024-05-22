﻿using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Events;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Domain.Aggregates;

public class StorageArea(StorageAreaId id) : AggregateRoot<StorageArea, StorageAreaId>(id)
{
    private readonly LinkedList<Product> _products = new();

    public IReadOnlyCollection<Product> Products => _products;

    public required StorageAreaName Name { get; init; }

    public OneOf<Success, DuplicateProductError> Add(Product item, TimeProvider dateTimeService)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(dateTimeService);

        if (_products.Contains(item))
        {
            return new DuplicateProductError(item);
        }

        _products.AddLast(item);
        AddDomainEvent(new ProductAddedEvent(dateTimeService, this, item));
        return new Success();
    }

    public OneOf<Success, NotFound> Remove(Product item, TimeProvider dateTimeService)
    {
        ArgumentNullException.ThrowIfNull(item);
        ArgumentNullException.ThrowIfNull(dateTimeService);

        if (!_products.Contains(item))
        {
            return new NotFound();
        }

        _products.Remove(item);
        AddDomainEvent(new ProductRemovedEvent(dateTimeService, this, item));
        return new Success();
    }
}