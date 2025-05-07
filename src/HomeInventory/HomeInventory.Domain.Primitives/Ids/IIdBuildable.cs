namespace HomeInventory.Domain.Primitives.Ids;

public interface IIdBuildable<out TSelf, in TId, TBuilder>
    where TSelf : class, IIdBuildable<TSelf, TId, TBuilder>, IBuildableIdentifierObject<TSelf, TId, TBuilder>, IValuableIdentifierObject<TSelf, TId>
    where TBuilder : IObjectBuilder<TSelf>
    where TId : notnull
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "As designed")]
    static abstract TSelf CreateFrom(TId value);
}
