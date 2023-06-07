using HomeInventory.Domain.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : notnull
{
    private static readonly string _requestName = typeof(TRequest).GetFormattedName();
    private static readonly string _responseName = typeof(TResponse).GetFormattedName();

    private readonly ILogger _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("Got {Request} and will return {Response}", _requestName, _responseName);
        _logger.LogInformation("Sending {Request}", request);
        var response = await next();

        HandleResponse(response);

        return response;
    }

    private void HandleResponse(TResponse? response)
    {
        if (response is IOneOf oneof)
        {
            switch (oneof.Index)
            {
                case 0:
                    _logger.LogInformation("{Value} was returned", oneof.Value);
                    break;
                case 1:
                    _logger.LogWarning("{Error} was returned", oneof.Value);
                    break;
            }
        }
    }
}
