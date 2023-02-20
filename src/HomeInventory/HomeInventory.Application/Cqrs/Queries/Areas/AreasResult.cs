using HomeInventory.Domain.Aggregates;

namespace HomeInventory.Application.Cqrs.Queries.Areas;

public record class AreasResult(IReadOnlyCollection<StorageArea> Areas);
