using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.Errors;

public record DuplicateProductError(Product Item) : ConflictError($"Duplicate product {Item.Name}");
