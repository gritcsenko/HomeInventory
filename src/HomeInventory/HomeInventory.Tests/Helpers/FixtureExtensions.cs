using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Helpers;

internal static class FixtureExtensions
{
    public static IFixture CustomizeGuidId<TId>(this IFixture fixture, Func<Guid, TId> createFunc) => fixture.CustomizeFromFactory(createFunc);

    public static IFixture CustomizeEmail(this IFixture fixture) => fixture.CustomizeString(value => new Email(value));

    public static IFixture CustomizeString<TValue>(this IFixture fixture, Func<string, TValue> createFunc) => fixture.CustomizeFromFactory((string value) => createFunc(value));

    public static IFixture CustomizeFromFactory<TValue, TObject>(this IFixture fixture, Func<TValue, TObject> createFunc) => fixture.Customize(new FromFactoryCustomization<TValue, TObject>(createFunc));
}
