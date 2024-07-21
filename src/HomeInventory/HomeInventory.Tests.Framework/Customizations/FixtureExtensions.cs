using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Framework.Customizations;

public static class FixtureExtensions
{
    public static IFixture CustomizeId<TId>(this IFixture fixture)
        where TId : class, IUlidBuildable<TId>, IUlidIdentifierObject<TId>, IValuableIdentifierObject<TId, Ulid> =>
        fixture
            .CustomizeUlid()
            .CustomizeFromFactory<TId, ISupplier<Ulid>>(s => TId.CreateBuilder().WithValue(s.Invoke()).Build().Value);

    public static IFixture CustomizeEmail(this IFixture fixture) =>
        fixture
            .CustomizeUlid()
            .CustomizeFromFactory<Email, ISupplier<Ulid>>(s => new Email(s.Invoke().ToString() + "@email.com"));

    public static IFixture CustomizeFromFactory<TObject>(this IFixture fixture, Func<TObject> createFunc)
    {
        fixture
            .Customize<TObject>(c => c.FromFactory(createFunc));
        return fixture;
    }

    public static IFixture CustomizeFromFactory<TObject, TValue>(this IFixture fixture, Func<TValue, TObject> createFunc)
    {
        fixture
            .Customize<TObject>(c => c.FromFactory(createFunc));
        return fixture;
    }

    public static IFixture CustomizeFromFactory<TObject, TValue1, TValue2>(this IFixture fixture, Func<TValue1, TValue2, TObject> createFunc)
    {
        fixture
            .Customize<TObject>(c => c.FromFactory(createFunc));
        return fixture;
    }

    public static IFixture CustomizeRegisterRequest(this IFixture fixture) =>
        fixture.Customize(new RegisterRequestCustomization());

    public static IFixture CustomizeUlid(this IFixture fixture) =>
        fixture.CustomizeIdSupply(IdSuppliers.Ulid);

    private static IFixture CustomizeIdSupply<TId>(this IFixture fixture, ISupplier<TId> supplier) =>
        fixture
            .CustomizeFromFactory(() => supplier)
            .CustomizeFromFactory<TId, ISupplier<TId>>(s => s.Invoke());
}
