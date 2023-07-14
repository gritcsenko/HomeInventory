using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Framework.Customizations;

public static class FixtureExtensions
{
    public static IFixture CustomizeUlidId<TId>(this IFixture fixture)
        where TId : class, IUlidIdentifierObject<TId> =>
        fixture.CustomizeFromFactory<Ulid, TId>(source => TId.CreateBuilder().WithValue(source).Invoke().Value);

    public static IFixture CustomizeEmail(this IFixture fixture) => fixture.CustomizeFromFactory<Ulid, Email>(value => new Email(value.ToString()));

    public static IFixture CustomizeString<TValue>(this IFixture fixture, Func<string, TValue> createFunc) => fixture.CustomizeFromFactory((string value) => createFunc(value));

    public static IFixture CustomizeFromFactory<TValue, TObject>(this IFixture fixture, Func<TValue, TObject> createFunc) =>
        fixture.Customize(new FromFactoryCustomization<TValue, TObject>(createFunc));

    public static IFixture CustomizeRegisterRequest(this IFixture fixture) =>
        fixture.Customize(new RegisterRequestCustomization());

    public static IFixture CustomizeUlid(this IFixture fixture) =>
        fixture.Customize(new UlidCustomization());
}
