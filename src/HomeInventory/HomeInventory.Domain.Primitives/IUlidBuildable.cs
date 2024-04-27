namespace HomeInventory.Domain.Primitives;

public interface IUlidBuildable<TSelf>
    where TSelf : class, IUlidBuildable<TSelf>, IUlidIdentifierObject<TSelf>
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "As designed")]
    abstract static Result<TSelf> CreateFrom(Ulid value);
}