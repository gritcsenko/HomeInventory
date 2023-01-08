using AutoFixture;
using HomeInventory.Tests.Customizations;

namespace HomeInventory.Tests.Helpers;

internal static class FixtureExtensions
{
    public static IFixture CustomizeGuidId<TId>(this IFixture fixture, Func<Guid, TId> createFunc) => fixture.Customize(new FromFactoryCustomization<Guid, TId>(createFunc));
}
