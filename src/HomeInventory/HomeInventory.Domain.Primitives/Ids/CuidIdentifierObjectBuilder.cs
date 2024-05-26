namespace HomeInventory.Domain.Primitives.Ids;

public sealed class CuidIdentifierObjectBuilder<TObject>() : IdentifierObjectBuilder<CuidIdentifierObjectBuilder<TObject>, TObject, Cuid>
    where TObject : class, IIdBuildable<TObject, Cuid, CuidIdentifierObjectBuilder<TObject>>, IBuildableIdentifierObject<TObject, Cuid, CuidIdentifierObjectBuilder<TObject>>, IValuableIdentifierObject<TObject, Cuid>
{
    protected override bool IsIdValid(Cuid value) => value != Cuid.Empty;
}
