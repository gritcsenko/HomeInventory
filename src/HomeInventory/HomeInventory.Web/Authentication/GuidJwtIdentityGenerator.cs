namespace HomeInventory.Web.Authentication;

public class GuidJwtIdentityGenerator : IJwtIdentityGenerator
{
    public string GenerateNew() => Ulid.NewUlid().ToString();
}
