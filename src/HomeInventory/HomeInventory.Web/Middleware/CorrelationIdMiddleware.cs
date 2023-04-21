using HomeInventory.Web.Configuration.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace HomeInventory.Web.Middleware;

internal class CorrelationIdMiddleware : IMiddleware
{
    private readonly ICorrelationIdContainer _container;
    private readonly ILogger _logger;

    public CorrelationIdMiddleware(ICorrelationIdContainer container, ILogger<CorrelationIdMiddleware> logger)
    {
        _container = container;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        SetCorrelationIdFrom(context.Request);
        AddCorrelationIdTo(context.Response);
        await next(context);
    }

    private void SetCorrelationIdFrom(HttpRequest request)
    {
        if (request.Headers.TryGetValue(HeaderNames.CorrelationId, out var correlationId) && (string?)correlationId is { } id)
        {
            _container.CorrelationId = id;
        }
        else
        {
            _container.GenerateNew();
            _logger.LogInformation("New {CorrelationId} was generated", _container.CorrelationId);
        }
    }

    private void AddCorrelationIdTo(HttpResponse response)
    {
        response.OnStarting(() =>
        {
            _logger.LogInformation("{CorrelationId} was returned to the caller", _container.CorrelationId);
            response.Headers[HeaderNames.CorrelationId] = new StringValues(_container.CorrelationId);
            return Task.CompletedTask;
        });
    }
}
