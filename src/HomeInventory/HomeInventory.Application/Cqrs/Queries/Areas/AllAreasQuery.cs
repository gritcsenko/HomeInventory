using HomeInventory.Application.Interfaces.Messaging;

namespace HomeInventory.Application.Cqrs.Queries.Areas;

public record class AllAreasQuery(object? Dummy = null) : IQuery<AreasResult>;
