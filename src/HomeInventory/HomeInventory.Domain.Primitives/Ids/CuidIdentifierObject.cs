namespace HomeInventory.Domain.Primitives.Ids;

public abstract class CuidIdentifierObject<TSelf>(Cuid value) : BuildableIdentifierObject<TSelf, Cuid, CuidIdentifierObjectBuilder<TSelf>>(value), ICuidIdentifierObject<TSelf>
    where TSelf : CuidIdentifierObject<TSelf>, ICuidBuildable<TSelf>
{
    public override string ToString() => Value.ToString();
}
