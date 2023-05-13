using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, OneOf<TResponse, IError>>
     where TRequest : IRequest<OneOf<TResponse, IError>>
{
    private static readonly string _requestName = typeof(TRequest).GetFormattedName();
    private static readonly string _responseName = typeof(TResponse).GetFormattedName();

    private readonly ILogger _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<OneOf<TResponse, IError>> Handle(TRequest request, RequestHandlerDelegate<OneOf<TResponse, IError>> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Request} was sent and expecting {Response}", _requestName, _responseName);
        var response = await next();
        response.Switch(
            r => _logger.LogInformation("{Request} was handled and {Response} = {Value} was returned", _requestName, _responseName, r),
            error => _logger.LogWarning("{Request} was not handled and {Error} was returned", _requestName, error));

        return response;
    }
}
