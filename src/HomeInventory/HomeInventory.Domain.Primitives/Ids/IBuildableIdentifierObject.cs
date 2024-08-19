namespace HomeInventory.Domain.Primitives.Ids;

public interface IBuildableIdentifierObject<TSelf, TId, TBuilder> : IIdentifierObject<TSelf>, IBuildableObject<TSelf, TBuilder>
    where TSelf : class, IBuildableIdentifierObject<TSelf, TId, TBuilder>, IIdBuildable<TSelf, TId, TBuilder>, IValuableIdentifierObject<TSelf, TId>
    where TBuilder : notnull, IObjectBuilder<TSelf>
    where TId : notnull
{
}