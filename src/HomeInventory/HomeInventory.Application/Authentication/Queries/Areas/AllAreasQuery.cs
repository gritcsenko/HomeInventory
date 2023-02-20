using HomeInventory.Application.Interfaces.Messaging;

namespace HomeInventory.Application.Authentication.Queries.Areas;

public record class AllAreasQuery() : IQuery<AreasResult>;
