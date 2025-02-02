using System.Runtime.Serialization;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.Errors;

[DataContract]
public record DuplicateProductError(Product Item) : ConflictError($"Duplicate product {Item.Name}");
