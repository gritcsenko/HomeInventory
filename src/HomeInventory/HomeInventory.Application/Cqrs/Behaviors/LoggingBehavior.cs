using HomeInventory.Application.Interfaces.Messaging;
using LanguageExt;
using Unit = LanguageExt.Unit;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private static readonly string _requestName = typeof(TRequest).GetFormattedName();
    private static readonly string _responseName = typeof(TResponse).GetFormattedName();

    private readonly ILogger _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using var scope = _logger.LoggingBehaviorScope(_requestName, _responseName);
        _logger.SendingRequest(request);
        var response = await next();

        var consumer = GetResponseHandler(response);
        consumer.Invoke(_logger);

        return response;
    }

    private static Action<ILogger> GetResponseHandler(TResponse response) =>
        response switch
        {
            Option<Error> option when option.IsNone => l => l.ValueReturned(Unit.Default),
            Option<Error> option when option.IsSome => l => l.ErrorReturned(option),
            IQueryResult result when result.IsSuccess => l => l.ValueReturned(result.Success),
            IQueryResult result when result.IsFail => l => l.ErrorReturned(result.Fail),
            var unknown => l => l.UnknownReturned(unknown),
        };
}
