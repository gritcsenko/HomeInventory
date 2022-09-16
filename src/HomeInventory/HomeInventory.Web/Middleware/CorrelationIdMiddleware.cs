using HomeInventory.Web.Configuration.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace HomeInventory.Web.Middleware;
internal class CorrelationIdMiddleware : IMiddleware
{
    private readonly ICorrelationIdGenerator _generator;
    private readonly ILogger _logger;

    public CorrelationIdMiddleware(ICorrelationIdGenerator generator, ILogger<CorrelationIdMiddleware> logger)
    {
        _generator = generator;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = GetCorrelationId(context);
        AddCorrelationId(context, correlationId);
        await next(context);
    }

    private string GetCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderNames.CorrelationId, out var correlationId))
        {
            var idText = correlationId.ToString();
            if (idText is not null)
            {
                _generator.SetCorrelationId(idText);
                return idText;
            }
        }

        var newId = _generator.GetCorrelationId();

        _logger.LogInformation("New {CorrelationId} was generated", newId);

        return newId;
    }

    private void AddCorrelationId(HttpContext context, string correlationId)
    {
        context.Response.OnStarting(() =>
        {
            _logger.LogInformation("{CorrelationId} was returned to the caller", correlationId);
            context.Response.Headers.Add(HeaderNames.CorrelationId, new StringValues(correlationId));
            return Task.CompletedTask;
        });
    }
}
