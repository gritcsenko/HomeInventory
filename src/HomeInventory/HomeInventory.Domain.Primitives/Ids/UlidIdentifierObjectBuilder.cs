namespace HomeInventory.Domain.Primitives.Ids;

public sealed class UlidIdentifierObjectBuilder<TObject>() : IdentifierObjectBuilder<UlidIdentifierObjectBuilder<TObject>, TObject, Ulid>()
    where TObject : class, IIdBuildable<TObject, Ulid, UlidIdentifierObjectBuilder<TObject>>, IBuildableIdentifierObject<TObject, Ulid, UlidIdentifierObjectBuilder<TObject>>
{
    protected override bool IsIdValid(Ulid value) => value != Ulid.Empty;
}
