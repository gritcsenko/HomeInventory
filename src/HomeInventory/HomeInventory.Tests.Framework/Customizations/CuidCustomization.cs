using Visus.Cuid;

namespace HomeInventory.Tests.Framework.Customizations;

internal class CuidCustomization : ICustomization
{
    public void Customize(IFixture fixture) => fixture.Customize<Cuid>(c => c.FromFactory(() => Cuid.NewCuid()));
}
