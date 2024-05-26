using DotNext;
using Visus.Cuid;

namespace HomeInventory.Web.Authentication;

public class CuidJwtIdentityGenerator(ISupplier<Cuid> supplier) : IJwtIdentityGenerator
{
    private readonly ISupplier<Cuid> _supplier = supplier;

    public string GenerateNew() => _supplier.Invoke().ToString();
}
