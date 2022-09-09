using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public class StockId : GuidIdentifierObject<StockId>
{
    public StockId(Guid value)
        : base(value)
    {
    }
}
