namespace HomeInventory.Domain.Primitives.Ids;

public interface IIdBuildable<TSelf, TId, TBuilder>
    where TSelf : class, IIdBuildable<TSelf, TId, TBuilder>, IBuildableIdentifierObject<TSelf, TId, TBuilder>, IValuableIdentifierObject<TSelf, TId>
    where TBuilder : notnull, IOptionalBuilder<TSelf>, IResettable
    where TId : notnull
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "As designed")]
    abstract static Result<TSelf> CreateFrom(TId value);
}

