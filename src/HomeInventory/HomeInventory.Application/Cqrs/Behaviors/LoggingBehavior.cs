using HomeInventory.Domain.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, IOneOf>
     where TRequest : IBaseRequest
{
    private static readonly string RequestName = typeof(TRequest).GetFormattedName();
    private static readonly string ResponseName = typeof(TResponse).GetFormattedName();

    private readonly ILogger _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<IOneOf> Handle(TRequest request, RequestHandlerDelegate<IOneOf> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending {Request}", RequestName);

        var response = await next();
        _logger.LogInformation("{Request} was handled and {Response}[{Index}] = {Value} was returned", RequestName, ResponseName, response.Index, response.Value);
        return response;
    }
}
