namespace HomeInventory.Domain.Primitives.Ids;

public interface IBuildableIdentifierObject<TSelf, TId, TBuilder> : IIdentifierObject<TSelf>, IOptionalBuildable<TSelf, TBuilder>
    where TSelf : class, IBuildableIdentifierObject<TSelf, TId, TBuilder>, IIdBuildable<TSelf, TId, TBuilder>, IValuableIdentifierObject<TSelf, TId>
    where TBuilder : notnull, IOptionalBuilder<TSelf>, IResettable
    where TId : notnull
{
}