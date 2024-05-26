namespace HomeInventory.Domain.Primitives.Ids;

public abstract class UlidIdentifierObject<TSelf>(Ulid value) : BuildableIdentifierObject<TSelf, Ulid, UlidIdentifierObjectBuilder<TSelf>>(value), IUlidIdentifierObject<TSelf>
    where TSelf : UlidIdentifierObject<TSelf>, IUlidBuildable<TSelf>
{
    public override string ToString() => Value.ToString();
}
