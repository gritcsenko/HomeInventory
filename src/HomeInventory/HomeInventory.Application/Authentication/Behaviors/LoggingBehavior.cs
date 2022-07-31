using MediatR;
using Microsoft.Extensions.Logging;

namespace HomeInventory.Application.Authentication.Behaviors;

internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
{
    private readonly ILogger _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        _logger.LogInformation("{Request} was sent and expecting {Response}", typeof(TRequest).Name, typeof(TResponse).Name);
        var response = await next();
        _logger.LogInformation("{Request} was handlef and {Response} was returned", typeof(TRequest).Name, typeof(TResponse).Name);
        return response;
    }
}
