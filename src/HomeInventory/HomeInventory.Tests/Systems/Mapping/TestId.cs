using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Tests.Systems.Mapping;

public class TestId(Ulid value) : UlidIdentifierObject<TestId>(value), IUlidBuildable<TestId>
{
    public static TestId CreateFrom(Ulid value) => new(value);
}
