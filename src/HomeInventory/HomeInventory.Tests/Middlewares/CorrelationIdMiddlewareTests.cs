using HomeInventory.Web.Configuration.Interfaces;
using HomeInventory.Web.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace HomeInventory.Tests.Middlewares;

[Trait("Category", "Unit")]
public class CorrelationIdMiddlewareTests : BaseTest
{
    private readonly TestingLogger<CorrelationIdMiddleware> _logger = Substitute.For<TestingLogger<CorrelationIdMiddleware>>();
    private readonly ICorrelationIdContainer _container = Substitute.For<ICorrelationIdContainer>();
    private readonly DefaultHttpContext _httpContext = new DefaultHttpContext();
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
            var exception = new Exception();
            source.SetException(exception);
            return source.Task;
        }

        var sut = CreateSut();

        Func<Task> invocation = async () => await sut.InvokeAsync(_httpContext, next);

        await invocation.Should().ThrowExactlyAsync<Exception>();
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

    [Fact]
    public async Task InvokeAsync_Should_CreateCorrelationId_When_HeaderIsNotSet()
    {
        static Task next(HttpContext ctx) => Task.CompletedTask;
        var sut = CreateSut();

        await sut.InvokeAsync(_httpContext, next);

        _container.Received(1).GenerateNew();
    }

    [Fact]
    public async Task InvokeAsync_Should_AddCorrelationIdToResponse()
    {
        var expectedId = Fixture.Create<string>();
        _container.CorrelationId.Returns(expectedId);
        _httpResponseFeature
            .When(f => f.OnStarting(Arg.Any<Func<object, Task>>(), Arg.Any<object>()))
            .Do(async ci =>
            {
                var func = ci.Arg<Func<object, Task>>();
                var state = ci.Arg<object>();
                await func(state);
            });

        static Task next(HttpContext ctx) => Task.CompletedTask;
        var sut = CreateSut();

        await sut.InvokeAsync(_httpContext, next);

        _httpResponseFeature.Received(1).OnStarting(Arg.Any<Func<object, Task>>(), Arg.Any<object>());
        _httpResponseFeature.Headers[HeaderNames.CorrelationId].Should().BeEquivalentTo(new[] { expectedId });
    }

    private CorrelationIdMiddleware CreateSut()
    {
        return new(_container, _logger);
    }
}
