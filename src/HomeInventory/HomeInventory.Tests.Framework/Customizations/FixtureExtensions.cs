using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.ValueObjects;
using Visus.Cuid;

namespace HomeInventory.Tests.Framework.Customizations;

public static class FixtureExtensions
{
    public static IFixture CustomizeId<TId>(this IFixture fixture)
        where TId : class, ICuidBuildable<TId>, ICuidIdentifierObject<TId> =>
        fixture
            .CustomizeCuid()
            .CustomizeFromFactory<Cuid, TId>(source => TId.CreateBuilder().WithValue(source).Build().Value);

    public static IFixture CustomizeEmail(this IFixture fixture) =>
        fixture
            .CustomizeCuid()
            .CustomizeFromFactory<Cuid, Email>(value => new Email(value.ToString()));

    public static IFixture CustomizeSupplier<TValue>(this IFixture fixture, Func<TValue> createFunc) =>
        fixture.CustomizeFromFactory<TValue, ISupplier<TValue>>(_ => new ValueSupplier<TValue>(createFunc()));

    public static IFixture CustomizeSupplier<TValue>(this IFixture fixture) =>
        fixture.CustomizeFromFactory<TValue, ISupplier<TValue>>(value => new ValueSupplier<TValue>(value));

    public static IFixture CustomizeFromFactory<TValue, TObject>(this IFixture fixture, Func<TValue, TObject> createFunc) =>
        fixture.Customize(new FromFactoryCustomization<TValue, TObject>(createFunc));

    public static IFixture CustomizeRegisterRequest(this IFixture fixture) =>
        fixture.Customize(new RegisterRequestCustomization());

    public static IFixture CustomizeUlid(this IFixture fixture) =>
        fixture.Customize(new UlidCustomization());

    public static IFixture CustomizeCuid(this IFixture fixture) =>
        fixture.Customize(new CuidCustomization());
}
