namespace HomeInventory.Domain.Primitives.Ids;

public interface IUlidBuildable<TSelf> : IIdBuildable<TSelf, Ulid, UlidIdentifierObjectBuilder<TSelf>>
    where TSelf : class, IUlidBuildable<TSelf>, IUlidIdentifierObject<TSelf>
{
}
