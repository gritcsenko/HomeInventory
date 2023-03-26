using HomeInventory.Domain.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
{
    private static readonly string RequestName = typeof(TRequest).GetFormattedName();
    private static readonly string ResponseName = typeof(TResponse).GetFormattedName();

    private readonly ILogger _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Request} was sent and expecting {Response}", RequestName, ResponseName);
        var response = await next();
        _logger.LogInformation("{Request} was handled and {Response} was returned", RequestName, ResponseName);
        return response;
    }
}
