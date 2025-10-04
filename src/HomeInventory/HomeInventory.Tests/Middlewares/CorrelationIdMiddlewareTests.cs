using HomeInventory.Web.ErrorHandling;
using HomeInventory.Web.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace HomeInventory.Tests.Middlewares;

[UnitTest]
public class CorrelationIdMiddlewareTests : BaseTest
{
    private readonly TestingLogger<CorrelationIdMiddleware> _logger = Substitute.For<TestingLogger<CorrelationIdMiddleware>>();
    private readonly CorrelationIdContainer _container = new();
    private readonly DefaultHttpContext _httpContext = new();
    private readonly IHttpResponseFeature _httpResponseFeature = Substitute.For<IHttpResponseFeature>();

    public CorrelationIdMiddlewareTests() => _httpContext.Features.Set(_httpResponseFeature);

    [Fact]
    public async Task InvokeAsync_Should_CallNext()
    {
        HttpContext? actualContext = null;

        var sut = CreateSut();

        await sut.InvokeAsync(_httpContext, CaptureContext);

        actualContext.Should().BeSameAs(_httpContext);

        Task CaptureContext(HttpContext ctx)
        {
            actualContext = ctx;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task InvokeAsync_Should_AwaitNext()
    {
        var sut = CreateSut();

        Func<Task> invocation = async () => await sut.InvokeAsync(_httpContext, SetException);

        await invocation.Should().ThrowExactlyAsync<InvalidOperationException>();

        static Task SetException(HttpContext ctx)
        {
            var source = new TaskCompletionSource();
            var exception = new InvalidOperationException();
            source.SetException(exception);
            return source.Task;
        }
    }

    [Fact]
    public async Task InvokeAsync_Should_SetCorrelationIdFromHeaders()
    {
        var expectedId = Fixture.Create<string>();
        _httpContext.Features.Get<IHttpRequestFeature>()!.Headers[HeaderNames.CorrelationId] = expectedId;
        static Task next(HttpContext ctx) => Task.CompletedTask;
        var sut = CreateSut();

        await sut.InvokeAsync(_httpContext, next);

        _container.CorrelationId.Should().Be(expectedId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task InvokeAsync_Should_NotSetCorrelationIdFromHeaders_When_ValueIsWrong(string? unexpectedId)
    {
        _httpContext.Features.Get<IHttpRequestFeature>()!.Headers[HeaderNames.CorrelationId] = unexpectedId;
        var sut = CreateSut();

        await sut.InvokeAsync(_httpContext, Next);

        _container.CorrelationId.Should().NotBe(unexpectedId);
    }

    [Fact]
    public async Task InvokeAsync_Should_CreateCorrelationId_When_HeaderIsNotSet()
    {
        var sut = CreateSut();
        var original = _container.CorrelationId;

        await sut.InvokeAsync(_httpContext, Next);

        _container.CorrelationId.Should().NotBe(original);
    }

    [Fact]
    public async Task InvokeAsync_Should_AddCorrelationIdToResponse()
    {
        _httpResponseFeature
            .When(static f => f.OnStarting(Arg.Any<Func<object, Task>>(), Arg.Any<object>()))
            .Do(static ci =>
            {
                var func = ci.Arg<Func<object, Task>>();
                var state = ci.Arg<object>();
                func(state);
            });

        var sut = CreateSut();

        await sut.InvokeAsync(_httpContext, Next);

        _httpResponseFeature.Received(1).OnStarting(Arg.Any<Func<object, Task>>(), Arg.Any<object>());
        _httpResponseFeature.Headers[HeaderNames.CorrelationId].Should().BeEquivalentTo(_container.CorrelationId);

    }

    private CorrelationIdMiddleware CreateSut() => new(_container, _logger);

    private static Task Next(HttpContext ctx) => Task.CompletedTask;
}
