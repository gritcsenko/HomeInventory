using HomeInventory.Application;

namespace HomeInventory.Tests.Application;

[UnitTest]
public sealed class HealthCheckStatusTests() : BaseTest<HealthCheckStatusTestsGivenContext>(static t => new(t))
{
    [Fact]
    public void Constructor_WithIsFailed_SetsIsFailed()
    {
        Given
            .New<bool>(out var expectedIsFailedVar)
            .New<string>(out var descriptionVar)
            .Status(out var statusVar, expectedIsFailedVar, descriptionVar);

        var then = When
            .Invoked(statusVar, static status => status.IsFailed);

        then
            .Result(expectedIsFailedVar, static (result, expected) => result.Should().Be(expected));
    }

    [Fact]
    public void Constructor_WithDescription_SetsDescription()
    {
        Given
            .New<bool>(out var isFailedVar)
            .New<string>(out var expectedDescriptionVar)
            .Status(out var statusVar, isFailedVar, expectedDescriptionVar);

        var then = When
            .Invoked(statusVar, static status => status.Description);

        then
            .Result(expectedDescriptionVar, static (result, expected) => result.Should().Be(expected));
    }

    [Fact]
    public void Constructor_WhenCalled_InitializesDataAsEmpty()
    {
        Given
            .New<bool>(out var isFailedVar)
            .New<string>(out var descriptionVar)
            .Status(out var statusVar, isFailedVar, descriptionVar);

        var then = When
            .Invoked(statusVar, static status => status.Data);

        then
            .Result(static result => result.Should().BeEmpty());
    }

    [Fact]
    public void Data_CanBePopulated()
    {
        Given
            .New<bool>(out var isFailedVar)
            .New<string>(out var descriptionVar)
            .New<string>(out var expectedKeyVar)
            .New<string>(out var expectedValueVar)
            .StatusWithData(out var statusVar, isFailedVar, descriptionVar, expectedKeyVar, expectedValueVar);

        var then = When
            .Invoked(statusVar, static status => status);

        then
            .Result(expectedKeyVar, expectedValueVar, static (result, expectedKey, expectedValue) =>
                result.Data.Should().ContainKey(expectedKey).WhoseValue.Should().Be(expectedValue));
    }
}

public sealed class HealthCheckStatusTestsGivenContext(BaseTest test) : GivenContext<HealthCheckStatusTestsGivenContext>(test)
{
    public HealthCheckStatusTestsGivenContext Status(
        out IVariable<HealthCheckStatus> statusVar,
        IVariable<bool> isFailedVar,
        IVariable<string> descriptionVar) =>
        New(out statusVar, isFailedVar, descriptionVar,
            static (isFailed, description) => new()
            {
                IsFailed = isFailed,
                Description = description,
            });

    public HealthCheckStatusTestsGivenContext StatusWithData(
        out IVariable<HealthCheckStatus> statusVar,
        IVariable<bool> isFailedVar,
        IVariable<string> descriptionVar,
        IVariable<string> keyVar,
        IVariable<string> valueVar) =>
        New(out statusVar, isFailedVar, descriptionVar, keyVar, valueVar,
            static (isFailed, description, key, value) => new()
            {
                IsFailed = isFailed,
                Description = description,
                Data =
                {
                    [key] = value,
                },
            });
}

