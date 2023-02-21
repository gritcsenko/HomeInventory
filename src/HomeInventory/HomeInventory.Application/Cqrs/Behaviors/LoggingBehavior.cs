using FluentResults;
using HomeInventory.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, IResult<TResponse>>
    where TRequest : IRequest<IResult<TResponse>>
{
    private static readonly string RequestName = typeof(TRequest).GetFormattedName();
    private static readonly string ResponseName = typeof(TResponse).GetFormattedName();

    private readonly ILogger _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<IResult<TResponse>> Handle(TRequest request, RequestHandlerDelegate<IResult<TResponse>> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending {Request} and expecting {Response}", RequestName, ResponseName);

        var response = await next();
        if (response.IsFailed)
        {
            _logger.LogError("{@Request} was failed with error(s): {@Error}", RequestName, string.Join("; ", response.Errors.Select(e => e.Message)));
        }
        else
        {
            _logger.LogInformation("{Request} was handled and {Response} was returned", RequestName, ResponseName);
        }
        return response;
    }
}
