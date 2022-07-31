namespace HomeInventory.Infrastructure.Authentication;

public class GuidJwtIdentityGenerator : IJwtIdentityGenerator
{
    public string GenerateNew() => Guid.NewGuid().ToString();
}
