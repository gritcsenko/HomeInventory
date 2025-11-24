using HomeInventory.Application;

namespace HomeInventory.Tests.Application;

[UnitTest]
public sealed class HealthCheckTagsTests() : BaseTest<HealthCheckTagsTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void Ready_WhenAccessed_ReturnsNonEmptyString()
    {
        Given
            .New(out var tagVar, static () => HealthCheckTags.Ready);

        var then = When
            .Invoked(tagVar, static tag => tag);

        then
            .Result(static result => result.Should().Be(nameof(HealthCheckTags.Ready)));
    }
}

public sealed class HealthCheckTagsTestsGivenContext(BaseTest test) : GivenContext<HealthCheckTagsTestsGivenContext>(test);

