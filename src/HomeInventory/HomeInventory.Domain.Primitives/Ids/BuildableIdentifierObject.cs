namespace HomeInventory.Domain.Primitives.Ids;

public abstract class BuildableIdentifierObject<TSelf, TId, TBuilder>(TId value) : IdentifierObject<TSelf, TId>(value), IBuildableIdentifierObject<TSelf, TId, TBuilder>
    where TSelf : BuildableIdentifierObject<TSelf, TId, TBuilder>, IIdBuildable<TSelf, TId, TBuilder>, IValuableIdentifierObject<TSelf, TId>
    where TId : notnull
    where TBuilder : IdentifierObjectBuilder<TBuilder, TSelf, TId>, IOptionalBuilder<TSelf>, IResettable, new()
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "Interface implementation")]
    public static TBuilder CreateBuilder() => new();
}
