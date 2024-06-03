namespace HomeInventory.Domain.Primitives.Ids;

public abstract class CuidIdentifierObject<TSelf>(Cuid value) : BuildableIdentifierObject<TSelf, Cuid, CuidIdentifierObjectBuilder<TSelf>>(value), ICuidIdentifierObject<TSelf>
    where TSelf : CuidIdentifierObject<TSelf>, ICuidBuildable<TSelf>
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "By design")]
    public static ISupplier<Cuid> IdSupplier => IdSuppliers.Cuid;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "By design")]
    public static ObjectConverter<Cuid, TSelf> Converter { get; } = new CuidIdConverter<TSelf>();

    public override string ToString() => Value.ToString();
}
