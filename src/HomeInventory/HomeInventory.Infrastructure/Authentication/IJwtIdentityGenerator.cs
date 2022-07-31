namespace HomeInventory.Infrastructure.Authentication;

public interface IJwtIdentityGenerator
{
    string GenerateNew();
}
