namespace HomeInventory.Domain.ValueObjects;
public class ProductId : GuidIdentifierObject<ProductId>
{
    internal ProductId(Guid value)
        : base(value)
    {
    }

    public static explicit operator Guid(ProductId obj) => obj.Value;
}

public interface IProductIdFactory
{
    ProductId CreateNew();
}

internal class ProductIdFactory : ValueObjectFactory<ProductId>, IProductIdFactory
{
    public ProductId CreateNew() => new(Guid.NewGuid());
}
