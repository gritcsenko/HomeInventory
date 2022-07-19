using AutoFixture;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Customizations;
internal class UserIdCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<UserId>(c => c.FromFactory(() => new UserId(Guid.NewGuid())));
    }
}
