using HomeInventory.Contracts;

namespace HomeInventory.Tests;

internal class RegisterRequestCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<RegisterRequest>(c => c
            .With(r => r.Email, () => $"{nameof(RegisterRequest.Email)}{fixture.Create<string>()}@email.com"));
    }
}
