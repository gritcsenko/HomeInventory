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
        _logger.LogInformation("{Request} was sent and expecting {Response}", _requestName, _responseName);
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
                    _logger.LogInformation("{Request} was handled and {Response} = {Value} was returned", _requestName, _responseName, oneof.Value);
                    break;
                case 1:
                    _logger.LogWarning("{Request} was not handled and {Error} was returned", _requestName, oneof.Value);
                    break;
            }
        }
    }
}
