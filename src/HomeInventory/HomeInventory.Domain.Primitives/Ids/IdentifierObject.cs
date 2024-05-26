namespace HomeInventory.Domain.Primitives.Ids;

public abstract class IdentifierObject<TSelf, TId>(TId value) : ValueObject<TSelf>(value), IIdentifierObject<TSelf>
    where TSelf : IdentifierObject<TSelf, TId>, IValuableIdentifierObject<TSelf, TId>
    where TId : notnull
{
    public TId Value => GetComponent<TId>(0);

    public override string? ToString() => Value.ToString();
}
