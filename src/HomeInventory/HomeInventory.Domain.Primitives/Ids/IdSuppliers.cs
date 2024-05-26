namespace HomeInventory.Domain.Primitives.Ids;

public static class IdSuppliers
{
    public static ISupplier<Cuid> Cuid { get; } = new DelegatingSupplier<Cuid>(Visus.Cuid.Cuid.NewCuid);
    public static ISupplier<Ulid> Ulid { get; } = new DelegatingSupplier<Ulid>(System.Ulid.NewUlid);
}
