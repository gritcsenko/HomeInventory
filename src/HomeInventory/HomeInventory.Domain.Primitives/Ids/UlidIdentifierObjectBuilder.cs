namespace HomeInventory.Domain.Primitives.Ids;

public sealed class UlidIdentifierObjectBuilder<TObject> : IdentifierObjectBuilder<UlidIdentifierObjectBuilder<TObject>, TObject, Ulid>
    where TObject : class, IIdBuildable<TObject, Ulid, UlidIdentifierObjectBuilder<TObject>>, IBuildableIdentifierObject<TObject, Ulid, UlidIdentifierObjectBuilder<TObject>>, IValuableIdentifierObject<TObject, Ulid>
{
    public UlidIdentifierObjectBuilder<TObject> WithNewId() => WithNewId(TObject.IdSupplier);

    public UlidIdentifierObjectBuilder<TObject> WithNewId(IIdSupplier<Ulid> supplier) => WithValue(supplier.Supply());

    protected override bool IsIdValid(Ulid value) => value != Ulid.Empty;
}
