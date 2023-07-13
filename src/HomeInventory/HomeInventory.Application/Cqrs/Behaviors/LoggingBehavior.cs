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
            IOneOf oneOf when oneOf.Index == 0 => l => l.ValueReturned(oneOf.Value),
            IOneOf oneOf when oneOf.Index == 1 => l => l.ErrorReturned(oneOf.Value),
            var unknown => l => l.UnknownReturned(unknown),
        };
}
