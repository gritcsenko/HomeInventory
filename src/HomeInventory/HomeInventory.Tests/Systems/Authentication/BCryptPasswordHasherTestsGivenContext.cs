using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Infrastructure.UserManagement.Services;

namespace HomeInventory.Tests.Systems.Authentication;

public sealed class BCryptPasswordHasherTestsGivenContext(BaseTest test) : GivenContext<BCryptPasswordHasherTestsGivenContext, IPasswordHasher>(test)
{
    protected override IPasswordHasher CreateSut() => new BCryptPasswordHasher() { WorkFactor = 6 };
}
