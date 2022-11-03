using HomeInventory.Domain.Entities;

namespace HomeInventory.Domain.Errors;

public class DuplicateProductError : ConflictError
{
    public DuplicateProductError(Product item)
        : base($"Duplicate product {item.Name}")
    {
        Item = item;
    }

    public Product Item { get; }
}
