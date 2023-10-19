namespace HomeInventory.Application.Mapping;

public class UlidIdConverter<TId> : BuilderObjectConverter<UlidIdentifierObjectBuilder<TId>, TId, Ulid>
    where TId : class, IUlidBuildable<TId>, IUlidIdentifierObject<TId>
{
}
