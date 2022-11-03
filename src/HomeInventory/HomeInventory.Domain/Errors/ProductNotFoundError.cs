using HomeInventory.Domain.Entities;

namespace HomeInventory.Domain.Errors;

public class ProductNotFoundError : NotFoundError
{
    public ProductNotFoundError(Product product)
        : base($"Product {product.Name} not found")
    {
    }
}
