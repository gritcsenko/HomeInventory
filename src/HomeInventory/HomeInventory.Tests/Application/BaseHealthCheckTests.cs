using HomeInventory.Application;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HomeInventory.Tests.Application;

[UnitTest]
public sealed class BaseHealthCheckTests() : BaseTest<BaseHealthCheckTestsGivenContext>(static t => new(t))
{
    [Fact]
    public async Task CheckHealthAsync_WhenHealthy_ReturnsHealthyStatus()
    {
        Given
            .New<string>(out var descriptionVar)
            .HealthCheckContext(out var contextVar)
            .Sut(out var sutVar, descriptionVar, isFailed: false);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, ctx, ct) => sut.CheckHealthAsync(ctx, ct));

        then
            .Result(descriptionVar, static (result, expectedDescription) =>
            {
                result.Status.Should().Be(HealthStatus.Healthy);
                result.Description.Should().Be(expectedDescription);
                result.Exception.Should().BeNull();
            });
    }

    [Fact]
    public async Task CheckHealthAsync_WhenFailed_ReturnsFailureStatus()
    {
        Given
            .New<string>(out var descriptionVar)
            .HealthCheckContext(out var contextVar, failureStatus: HealthStatus.Degraded)
            .Sut(out var sutVar, descriptionVar, isFailed: true);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, ctx, ct) => sut.CheckHealthAsync(ctx, ct));

        then
            .Result(descriptionVar, static (result, expectedDescription) =>
            {
                result.Status.Should().Be(HealthStatus.Degraded);
                result.Description.Should().Be(expectedDescription);
                result.Exception.Should().BeNull();
            });
    }

    [Fact]
    public async Task CheckHealthAsync_WhenExceptionThrown_ReturnsFailureWithException()
    {
        Given
            .New<Exception>(out var exceptionVar, static () => new InvalidOperationException())
            .HealthCheckContext(out var contextVar, failureStatus: HealthStatus.Unhealthy)
            .Sut(out var sutVar, exceptionVar);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, ctx, ct) => sut.CheckHealthAsync(ctx, ct));

        then
            .Result(exceptionVar, static (result, expectedException) =>
            {
                result.Status.Should().Be(HealthStatus.Unhealthy);
                result.Description.Should().Be("Failed to perform healthcheck");
                result.Exception.Should().BeSameAs(expectedException);
            });
    }

    [Fact]
    public async Task CheckHealthAsync_WhenHealthyWithData_ReturnsDataInResult()
    {
        Given
            .New<string>(out var descriptionVar)
            .New<string>(out var dataKeyVar)
            .New<string>(out var dataValueVar)
            .HealthCheckContext(out var contextVar)
            .Sut(out var sutVar, descriptionVar, dataKeyVar, dataValueVar);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, ctx, ct) => sut.CheckHealthAsync(ctx, ct));

        then
            .Result(dataKeyVar, dataValueVar, static (result, key, value) =>
            {
                result.Status.Should().Be(HealthStatus.Healthy);
                result.Data.Should().ContainKey(key).WhoseValue.Should().Be(value);
            });
    }

    [Fact]
    public async Task CheckHealthAsync_WhenExceptionWithCustomData_ReturnsCustomExceptionData()
    {
        Given
            .New<Exception>(out var exceptionVar, static () => new InvalidOperationException())
            .New<string>(out var dataKeyVar)
            .New<string>(out var dataValueVar)
            .HealthCheckContext(out var contextVar, failureStatus: HealthStatus.Unhealthy)
            .SutWithExceptionData(out var sutVar, exceptionVar, dataKeyVar, dataValueVar);

        var then = await When
            .InvokedAsync(sutVar, contextVar, static (sut, ctx, ct) => sut.CheckHealthAsync(ctx, ct));

        then
            .Result(exceptionVar, dataKeyVar, dataValueVar, static (result, expectedException, key, value) =>
            {
                result.Status.Should().Be(HealthStatus.Unhealthy);
                result.Data.Should().ContainKey(key).WhoseValue.Should().Be(value);
                result.Exception.Should().BeSameAs(expectedException);
            });
    }
}

public sealed class BaseHealthCheckTestsGivenContext(BaseTest test) : GivenContext<BaseHealthCheckTestsGivenContext>(test)
{
    public BaseHealthCheckTestsGivenContext HealthCheckContext(
        out IVariable<HealthCheckContext> contextVar,
        HealthStatus failureStatus = HealthStatus.Unhealthy) =>
        New(out contextVar, () => new()
        {
            Registration = new(
                Create<string>(),
                Substitute.For<IHealthCheck>(),
                failureStatus,
                []),
        });

    public BaseHealthCheckTestsGivenContext Sut(
        out IVariable<TestHealthCheck> sutVar,
        IVariable<string> descriptionVar,
        bool isFailed) =>
        New(out sutVar, descriptionVar, desc => new(desc, isFailed));

    public BaseHealthCheckTestsGivenContext Sut(
        out IVariable<TestHealthCheck> sutVar,
        IVariable<Exception> exceptionVar) =>
        New(out sutVar, exceptionVar, static ex => new(ex));

    public BaseHealthCheckTestsGivenContext Sut(
        out IVariable<TestHealthCheck> sutVar,
        IVariable<string> descriptionVar,
        IVariable<string> dataKeyVar,
        IVariable<string> dataValueVar) =>
        New(out sutVar, descriptionVar, dataKeyVar, dataValueVar,
            static (desc, key, value) => new(desc, false, key, value));

    public BaseHealthCheckTestsGivenContext SutWithExceptionData(
        out IVariable<TestHealthCheck> sutVar,
        IVariable<Exception> exceptionVar,
        IVariable<string> dataKeyVar,
        IVariable<string> dataValueVar) =>
        New(out sutVar, exceptionVar, dataKeyVar, dataValueVar,
            static (ex, key, value) => new(ex, key, value));
}

public sealed class TestHealthCheck : BaseHealthCheck
{
    private readonly string? _description;
    private readonly bool _isFailed;
    private readonly Exception? _exception;
    private readonly string? _dataKey;
    private readonly object? _dataValue;
    private readonly Dictionary<string, object>? _exceptionData;

    public TestHealthCheck(string description, bool isFailed, string? dataKey = null, object? dataValue = null)
    {
        _description = description;
        _isFailed = isFailed;
        _dataKey = dataKey;
        _dataValue = dataValue;
    }

    public TestHealthCheck(Exception exception, string? dataKey = null, object? dataValue = null)
    {
        _exception = exception;
        _exceptionData = dataKey != null && dataValue != null
            ? new Dictionary<string, object> { [dataKey] = dataValue }
            : null;
    }

    protected override ValueTask<HealthCheckStatus> CheckHealthAsync(CancellationToken cancellationToken)
    {
        if (_exception is not null)
        {
            throw _exception;
        }

        var status = new HealthCheckStatus
        {
            IsFailed = _isFailed,
            Description = _description!,
        };

        if (_dataKey is not null && _dataValue is not null)
        {
            status.Data[_dataKey] = _dataValue;
        }

        return ValueTask.FromResult(status);
    }

    protected override IReadOnlyDictionary<string, object> ExceptionData =>
        _exceptionData ?? base.ExceptionData;
}

