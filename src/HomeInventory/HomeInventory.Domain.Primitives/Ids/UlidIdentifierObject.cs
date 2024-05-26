
using HomeInventory.Application.Mapping;

namespace HomeInventory.Domain.Primitives.Ids;

public abstract class UlidIdentifierObject<TSelf>(Ulid value) : BuildableIdentifierObject<TSelf, Ulid, UlidIdentifierObjectBuilder<TSelf>>(value), IUlidIdentifierObject<TSelf>, IValuableIdentifierObject<TSelf, Ulid>
    where TSelf : UlidIdentifierObject<TSelf>, IUlidBuildable<TSelf>
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "By design")]
    public static ISupplier<Ulid> IdSupplier => IdSuppliers.Ulid;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types", Justification = "By design")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "By design")]
    public static ObjectConverter<Ulid, TSelf> Converter { get; } = new UlidIdConverter<TSelf>();

    public override string ToString() => Value.ToString();
}
