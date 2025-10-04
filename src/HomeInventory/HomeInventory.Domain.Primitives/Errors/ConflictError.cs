using System.Runtime.Serialization;

namespace HomeInventory.Domain.Primitives.Errors;

[DataContract]
public record ConflictError(string Message) : Exceptional(Message, -1_000_000_003);
