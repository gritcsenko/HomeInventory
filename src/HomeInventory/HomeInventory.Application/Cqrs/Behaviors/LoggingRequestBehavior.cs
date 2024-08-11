using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal sealed class LoggingRequestBehavior<TRequest, TResponse>(ILogger<LoggingRequestBehavior<TRequest, TResponse>> logger) : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequestMessage<TResponse>
{
    private static readonly string _requestName = typeof(TRequest).GetFormattedName();
    private static readonly string _responseName = typeof(TResponse).GetFormattedName();

    private readonly ILogger _logger = logger;

    public async Task<TResponse> OnRequestAsync(IRequestContext<TRequest> context, Func<IRequestContext<TRequest>, Task<TResponse>> handler)
    {
        using var scope = _logger.LoggingBehaviorScope(_requestName, _responseName);
        _logger.SendingRequest(context.Request);
        var response = await handler(context);

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
