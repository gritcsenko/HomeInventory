using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Framework.Customizations;

public static class FixtureExtensions
{
    public static IFixture CustomizeId<TId>(this IFixture fixture)
        where TId : class, IUlidBuildable<TId>, IUlidIdentifierObject<TId>, IValuableIdentifierObject<TId, Ulid> =>
        fixture
            .CustomizeUlid()
            .CustomizeFromFactory<TId, IIdSupplier<Ulid>>(s => (TId)TId.CreateBuilder().WithValue(s.Supply()).Build());

    public static IFixture CustomizeId<TId>(this IFixture fixture, DateTimeOffset timeStamp, Random? random = null)
        where TId : class, IUlidBuildable<TId>, IUlidIdentifierObject<TId>, IValuableIdentifierObject<TId, Ulid> =>
        fixture
            .CustomizeUlid(timeStamp, random)
            .CustomizeFromFactory<TId, IIdSupplier<Ulid>>(s => (TId)TId.CreateBuilder().WithValue(s.Supply()).Build());

    public static IFixture CustomizeEmail(this IFixture fixture) =>
        fixture
            .CustomizeUlid()
            .CustomizeFromFactory<Email, IIdSupplier<Ulid>>(s => new Email(s.Supply().ToString() + "@email.com"));

    public static IFixture CustomizeEmail(this IFixture fixture, DateTimeOffset timeStamp, Random? random = null) =>
        fixture
            .CustomizeUlid(timeStamp, random)
            .CustomizeFromFactory<Email, IIdSupplier<Ulid>>(s => new Email(s.Supply().ToString() + "@email.com"));

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
        fixture.CustomizeIdSupply(PredictibleUlidSupplier.Instance);

    public static IFixture CustomizeUlid(this IFixture fixture, DateTimeOffset timeStamp, Random? random = null) =>
        fixture.CustomizeIdSupply(new PredictibleUlidSupplier(timeStamp, random ?? new Random((int)(timeStamp.UtcTicks % int.MaxValue))));

    private static IFixture CustomizeIdSupply<TId>(this IFixture fixture, IIdSupplier<TId> supplier) =>
        fixture
            .CustomizeFromFactory(() => supplier)
            .CustomizeFromFactory<TId, IIdSupplier<TId>>(s => s.Supply());

    private sealed class PredictibleUlidSupplier(DateTimeOffset timeStamp, Random random) : IIdSupplier<Ulid>
    {
        private readonly DateTimeOffset _timeStamp = timeStamp;
        private readonly Random _random = random;

        public static IIdSupplier<Ulid> Instance { get; } = new PredictibleUlidSupplier(DateTimeOffset.MinValue, new Random());

        public static DateTimeOffset TimeStamp { get; } = DateTimeOffset.MinValue;

        public Ulid Supply()
        {
            Span<byte> bytes = stackalloc byte[10];
#pragma warning disable CA5394 // Do not use insecure randomness
            _random.NextBytes(bytes);
#pragma warning restore CA5394 // Do not use insecure randomness
            return Ulid.NewUlid(_timeStamp, bytes);
        }
    }
}
