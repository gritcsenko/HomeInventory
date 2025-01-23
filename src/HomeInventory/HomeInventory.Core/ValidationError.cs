using System.Runtime.Serialization;

namespace HomeInventory.Core;

[DataContract]
public record ValidationError(string Message, object? Value) : Exceptional(Message, -1_000_000_001)
{
    public override Exception ToException() => new ValidationException(this);
}
