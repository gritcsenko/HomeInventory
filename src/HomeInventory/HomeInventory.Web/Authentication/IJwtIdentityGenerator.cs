namespace HomeInventory.Web.Authentication;

public interface IJwtIdentityGenerator
{
    string GenerateNew();
}
