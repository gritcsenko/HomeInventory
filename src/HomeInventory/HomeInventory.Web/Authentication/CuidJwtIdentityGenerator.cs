using DotNext;

namespace HomeInventory.Web.Authentication;

public class CuidJwtIdentityGenerator(ISupplier<Ulid> supplier) : IJwtIdentityGenerator
{
    private readonly ISupplier<Ulid> _supplier = supplier;

    public string GenerateNew() => _supplier.Invoke().ToString();
}
