using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Web.Authentication;

public class CuidJwtIdentityGenerator(IIdSupplier<Ulid> supplier) : IJwtIdentityGenerator
{
    private readonly IIdSupplier<Ulid> _supplier = supplier;

    public string GenerateNew() => _supplier.Supply().ToString();
}
