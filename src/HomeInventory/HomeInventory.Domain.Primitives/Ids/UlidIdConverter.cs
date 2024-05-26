using HomeInventory.Application.Mapping;

namespace HomeInventory.Domain.Primitives.Ids;

public class UlidIdConverter<TId> : BuilderObjectConverter<UlidIdentifierObjectBuilder<TId>, TId, Ulid>
    where TId : class, IUlidBuildable<TId>, IUlidIdentifierObject<TId>, IValuableIdentifierObject<TId, Ulid>
{
}
