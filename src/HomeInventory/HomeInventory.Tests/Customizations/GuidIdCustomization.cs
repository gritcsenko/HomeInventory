using AutoFixture;

namespace HomeInventory.Tests.Customizations;

internal static class GuidIdCustomization
{
    public static ICustomization Create<TId>(Func<Guid, TId> createFunc) => new FromFactoryCustomization<Guid, TId>(createFunc);
}
