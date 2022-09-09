using HomeInventory.Domain.DomainEvents;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Aggregates;

public class Stock : AggregateRoot<Stock, StockId>
{
    private readonly List<Product> _products = new();

    public Stock(StockId id)
        : base(id)
    {
    }

    public IReadOnlyCollection<Product> Products => _products;

    public void AddProduct(Product product)
    {
        _products.Add(product);
        OnProductAdded(product);
    }

    private void OnProductAdded(Product product) => Publish(new ProductAddedToStockDomainEvent(product.Id, Id));
}
