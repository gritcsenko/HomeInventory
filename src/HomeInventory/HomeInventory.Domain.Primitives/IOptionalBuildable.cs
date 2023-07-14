namespace HomeInventory.Domain.Primitives;

public interface IOptionalBuildable<out TSelf, out TBuilder>
    where TSelf : notnull, IOptionalBuildable<TSelf, TBuilder>
    where TBuilder : notnull, ISupplier<Optional<TSelf>>, IResettable
{
    //
    // Summary:
    //     Creates a new builder for type TSelf.
    //
    // Returns:
    //     A new builder for type TSelf.
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "As designed")]
    abstract static TBuilder CreateBuilder();
}
