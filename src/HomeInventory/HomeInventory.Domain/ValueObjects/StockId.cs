using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class StockId : GuidIdentifierObject<StockId>
{
    public StockId(Guid value)
        : base(value)
    {
    }
}
