namespace HomeInventory.Tests.Framework.Customizations;

internal class UlidCustomization : ICustomization
{
    public void Customize(IFixture fixture) => fixture.Customize<Ulid>(c => c.FromFactory(() => Ulid.NewUlid()));
}
