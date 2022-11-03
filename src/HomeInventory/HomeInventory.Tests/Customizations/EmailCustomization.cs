using AutoFixture;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Customizations;

internal class EmailCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<Email>(c => c.FromFactory<string>(value => new Email(value)));
    }
}