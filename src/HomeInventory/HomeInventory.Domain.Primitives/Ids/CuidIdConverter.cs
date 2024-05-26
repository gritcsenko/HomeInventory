using HomeInventory.Application.Mapping;

namespace HomeInventory.Domain.Primitives.Ids;

public class CuidIdConverter<TId> : BuilderObjectConverter<CuidIdentifierObjectBuilder<TId>, TId, Cuid>
    where TId : class, ICuidBuildable<TId>, ICuidIdentifierObject<TId>
{
}