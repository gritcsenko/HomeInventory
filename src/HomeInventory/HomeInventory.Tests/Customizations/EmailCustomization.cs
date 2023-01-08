using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Customizations;

internal class EmailCustomization : FromFactoryCustomization<string, Email>
{
    public EmailCustomization()
        : base(value => new Email(value))
    {
    }
}