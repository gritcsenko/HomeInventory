using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Tests.Systems.Mapping;

public sealed class UlidIdConverterTestsGivenContext : GivenContext<UlidIdConverterTestsGivenContext, UlidIdConverter<TestId>>
{
    public UlidIdConverterTestsGivenContext(BaseTest test)
        : base(test) =>
        test.Fixture.CustomizeUlid();

    internal UlidIdConverterTestsGivenContext Empty(out IVariable<Ulid> empty) =>
        New(out empty, static () => Ulid.Empty);

    protected override UlidIdConverter<TestId> CreateSut() => new();
}
