using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, OneOf<TResponse, IError>>
     where TRequest : IRequest<OneOf<TResponse, IError>>
{
    private static readonly string RequestName = typeof(TRequest).GetFormattedName();
    private static readonly string ResponseName = typeof(TResponse).GetFormattedName();

    private readonly ILogger _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<OneOf<TResponse, IError>> Handle(TRequest request, RequestHandlerDelegate<OneOf<TResponse, IError>> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Request} was sent and expecting {Response}", RequestName, ResponseName);
        var response = await next();
        response.Switch(
            r => _logger.LogInformation("{Request} was handled and {Response} = {Value} was returned", RequestName, ResponseName, r),
            error => _logger.LogWarning("{Request} was not handled and {Error} was returned", RequestName, error));

        return response;
    }
}
