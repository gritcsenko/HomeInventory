namespace HomeInventory.Domain.Primitives.Ids;

public sealed class DelegatingIdSupplier<TId>(Func<TId> supplyFunc) : IIdSupplier<TId>
{
    private readonly Func<TId> _supplyFunc = supplyFunc;

    public TId Supply() => _supplyFunc();
}
