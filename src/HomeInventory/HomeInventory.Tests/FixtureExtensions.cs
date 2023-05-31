using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests;

internal static class FixtureExtensions
{
    public static IFixture CustomizeGuidId<TId>(this IFixture fixture)
        where TId : class, IGuidIdentifierObject<TId> =>
#pragma warning disable CA2252 // This API requires opting into preview features
        fixture.CustomizeFromFactory<Guid, TId>(source => TId.CreateBuilder().WithValue(new ValueSupplier<Guid>(source)).Invoke());
#pragma warning restore CA2252 // This API requires opting into preview features

    public static IFixture CustomizeEmail(this IFixture fixture) => fixture.CustomizeString(value => new Email(value));

    public static IFixture CustomizeString<TValue>(this IFixture fixture, Func<string, TValue> createFunc) => fixture.CustomizeFromFactory((string value) => createFunc(value));

    public static IFixture CustomizeFromFactory<TValue, TObject>(this IFixture fixture, Func<TValue, TObject> createFunc) => fixture.Customize(new FromFactoryCustomization<TValue, TObject>(createFunc));
}
