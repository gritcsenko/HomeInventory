using HomeInventory.Domain.Aggregates;

namespace HomeInventory.Application.Authentication.Queries.Areas;

public record class AreasResult(IReadOnlyCollection<StorageArea> Areas);
