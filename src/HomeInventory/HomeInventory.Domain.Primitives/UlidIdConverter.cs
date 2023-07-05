namespace HomeInventory.Application.Mapping;

public class UlidIdConverter<TId> : BuilderObjectConverter<UlidIdentifierObjectBuilder<TId>, TId, Ulid>
    where TId : class, IUlidIdentifierObject<TId>
{
}
