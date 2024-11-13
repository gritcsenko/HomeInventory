using HomeInventory.Web.Configuration;
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

    public CorrelationIdMiddlewareTests()
    {
        _httpContext.Features.Set(_httpResponseFeature);
    }

    [Fact]
    public async Task InvokeAsync_Should_CallNext()
    {
        HttpContext? actualContext = null;
        Task next(HttpContext ctx)
        {
            actualContext = ctx;
            return Task.CompletedTask;
        }

        var sut = CreateSut();

        await sut.InvokeAsync(_httpContext, next);

        actualContext.Should().BeSameAs(_httpContext);
    }

    [Fact]
    public async Task InvokeAsync_Should_AwaitNext()
    {
        static Task next(HttpContext ctx)
        {
            var source = new TaskCompletionSource();
            var exception = new InvalidOperationException();
            source.SetException(exception);
            return source.Task;
        }

        var sut = CreateSut();

        Func<Task> invocation = async () => await sut.InvokeAsync(_httpContext, next);

        await invocation.Should().ThrowExactlyAsync<InvalidOperationException>();
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
        static Task next(HttpContext ctx) => Task.CompletedTask;
        var sut = CreateSut();

        await sut.InvokeAsync(_httpContext, next);

        _container.CorrelationId.Should().NotBe(unexpectedId);
    }

    [Fact]
    public async Task InvokeAsync_Should_CreateCorrelationId_When_HeaderIsNotSet()
    {
        static Task next(HttpContext ctx) => Task.CompletedTask;
        var sut = CreateSut();
        var original = _container.CorrelationId;

        await sut.InvokeAsync(_httpContext, next);

        _container.CorrelationId.Should().NotBe(original);
    }

    [Fact]
    public async Task InvokeAsync_Should_AddCorrelationIdToResponse()
    {
        _httpResponseFeature
            .When(static f => f.OnStarting(Arg.Any<Func<object, Task>>(), Arg.Any<object>()))
            .Do(static async ci =>
            {
                var func = ci.Arg<Func<object, Task>>();
                var state = ci.Arg<object>();
                await func(state);
            });

        static Task next(HttpContext ctx) => Task.CompletedTask;
        var sut = CreateSut();

        await sut.InvokeAsync(_httpContext, next);

        _httpResponseFeature.Received(1).OnStarting(Arg.Any<Func<object, Task>>(), Arg.Any<object>());
        _httpResponseFeature.Headers[HeaderNames.CorrelationId].Should().BeEquivalentTo(_container.CorrelationId);
    }

    private CorrelationIdMiddleware CreateSut()
    {
        return new(_container, _logger);
    }
}
