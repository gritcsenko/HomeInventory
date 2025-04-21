using HomeInventory.Application.Framework.Messaging;
using Unit = LanguageExt.Unit;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly string _requestName = typeof(TRequest).GetFormattedName();
    private static readonly string _responseName = typeof(TResponse).GetFormattedName();

    private readonly ILogger _logger = logger;

    public async Task<TResponse> Handle(TRequest request, Func<CancellationToken, Task<TResponse>> next, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.LoggingBehaviorScope(_requestName, _responseName);
        _logger.SendingRequest(request);
        var response = await next(cancellationToken);

        var consumer = GetResponseHandler(response);
        consumer.Invoke(_logger);

        return response;
    }

    private static Action<ILogger> GetResponseHandler(TResponse response) =>
        response switch
        {
            Option<Error> { IsNone: true } => l => l.ValueReturned(Unit.Default),
            Option<Error> { IsSome: true } option => l => l.ErrorReturned(option),
            IQueryResult { IsSuccess: true } result => l => l.ValueReturned(result.Success),
            IQueryResult { IsFail: true } result => l => l.ErrorReturned(result.Fail),
            var unknown => l => l.UnknownReturned(unknown),
        };
}
