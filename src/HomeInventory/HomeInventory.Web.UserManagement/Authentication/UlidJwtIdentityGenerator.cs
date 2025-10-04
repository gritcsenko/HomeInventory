using HomeInventory.Application.UserManagement.Interfaces;

namespace HomeInventory.Web.UserManagement.Authentication;

public class UlidJwtIdentityGenerator : IJwtIdentityGenerator
{
    public string GenerateNew() => Ulid.NewUlid().ToString();
}
