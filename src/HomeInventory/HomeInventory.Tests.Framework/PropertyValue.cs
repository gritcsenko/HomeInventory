namespace HomeInventory.Tests.Framework;

public record struct PropertyValue<T>(T Value)
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Not needed")]
    public static implicit operator PropertyValue<T>(T value) => new(value);
}
