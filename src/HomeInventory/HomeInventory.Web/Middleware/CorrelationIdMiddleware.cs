using HomeInventory.Web.ErrorHandling.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HomeInventory.Web.Middleware;

internal class CorrelationIdMiddleware(ICorrelationIdContainer container, ILogger<CorrelationIdMiddleware> logger) : IMiddleware
{
    private readonly ICorrelationIdContainer _container = container;
    private readonly ILogger _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        SetCorrelationIdFrom(context.Request);
        AddCorrelationIdTo(context.Response);
        await next(context);
    }

    private void SetCorrelationIdFrom(HttpRequest request)
    {
        if (request.Headers.TryGetValue(HeaderNames.CorrelationId, out var correlationId) && correlationId.ToString() is { Length: > 0 } id)
        {
            _container.SetExisting(id);
        }
        else
        {
            _container.GenerateNew();
            _logger.CorrelationIdGenerated(_container.CorrelationId);
        }
    }

    private void AddCorrelationIdTo(HttpResponse response) =>
        response.OnStarting(() =>
        {
            _logger.CorrelationIdReturned(_container.CorrelationId);
            response.Headers[HeaderNames.CorrelationId] = new(_container.CorrelationId);
            return Task.CompletedTask;
        });
}
