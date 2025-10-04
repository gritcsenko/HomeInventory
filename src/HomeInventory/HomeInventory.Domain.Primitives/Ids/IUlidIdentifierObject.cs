namespace HomeInventory.Domain.Primitives.Ids;

public interface IUlidIdentifierObject<TSelf> : IBuildableIdentifierObject<TSelf, Ulid, UlidIdentifierObjectBuilder<TSelf>>
    where TSelf : class, IUlidIdentifierObject<TSelf>, IIdBuildable<TSelf, Ulid, UlidIdentifierObjectBuilder<TSelf>>, IValuableIdentifierObject<TSelf, Ulid>
{
}
