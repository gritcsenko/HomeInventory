namespace HomeInventory.Domain.Primitives.Ids;

public static class IdSuppliers
{
    public static IIdSupplier<Ulid> Ulid { get; } = new DelegatingIdSupplier<Ulid>(System.Ulid.NewUlid);
}
