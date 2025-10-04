using System.Runtime.Serialization;

namespace HomeInventory.Domain.Primitives.Errors;

[DataContract]
public record NotFoundError(string Message) : Exceptional(Message, -1_000_000_002);
