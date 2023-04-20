using HomeInventory.Domain.Entities;

namespace HomeInventory.Domain.Errors;

public record DuplicateProductError(Product Item) : ConflictError($"Duplicate product {Item.Name}");
