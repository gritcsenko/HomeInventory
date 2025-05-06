using System.Runtime.Serialization;

namespace HomeInventory.Domain.Errors;

[DataContract]
public record InvalidCredentialsError() : Exceptional("Invalid credentials", -1_000_000_005);
