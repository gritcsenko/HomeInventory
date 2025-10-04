using HomeInventory.Contracts.UserManagement;

namespace HomeInventory.Tests.Framework.Customizations;

internal class RegisterRequestCustomization : ICustomization
{
    public void Customize(IFixture fixture) =>
        fixture.Customize<RegisterRequest>(c => c
            .With(r => r.Email, () => $"{nameof(RegisterRequest.Email)}{fixture.Create<string>()}@email.com"));
}
