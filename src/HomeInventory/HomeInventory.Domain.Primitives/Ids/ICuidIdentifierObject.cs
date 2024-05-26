namespace HomeInventory.Domain.Primitives.Ids;

public interface ICuidIdentifierObject<TSelf> : IBuildableIdentifierObject<TSelf, Cuid, CuidIdentifierObjectBuilder<TSelf>>, IValuableIdentifierObject<TSelf, Cuid>
    where TSelf : class, ICuidIdentifierObject<TSelf>, IIdBuildable<TSelf, Cuid, CuidIdentifierObjectBuilder<TSelf>>
{
}
