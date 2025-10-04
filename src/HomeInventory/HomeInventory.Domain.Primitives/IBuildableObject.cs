namespace HomeInventory.Domain.Primitives;

public interface IBuildableObject<out TSelf, out TBuilder>
    where TSelf : IBuildableObject<TSelf, TBuilder>
    where TBuilder : IObjectBuilder<TSelf>
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "As designed")]
    static abstract TBuilder CreateBuilder();
}
