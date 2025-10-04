using System.Runtime.Serialization;

namespace HomeInventory.Domain.Primitives.Errors;

[DataContract]
public record ValueNotSpecifiedError() : Exceptional("Value was not specified", -1_000_000_004);
