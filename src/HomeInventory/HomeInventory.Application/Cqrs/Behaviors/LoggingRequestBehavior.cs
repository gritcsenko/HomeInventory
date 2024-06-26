using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal sealed class LoggingRequestBehavior<TRequest, TResponse>(ILogger<LoggingRequestBehavior<TRequest, TResponse>> logger) : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequestMessage<TResponse>
{
    private static readonly string _requestName = typeof(TRequest).GetFormattedName();
    private static readonly string _responseName = typeof(TResponse).GetFormattedName();

    private readonly ILogger _logger = logger;

    public async Task<OneOf<TResponse, IError>> OnRequest(IMessageHub hub, TRequest request, Func<Task<OneOf<TResponse, IError>>> handler, CancellationToken cancellationToken = default)
    {
        using var scope = _logger.LoggingBehaviorScope(_requestName, _responseName);
        _logger.SendingRequest(request);
        var response = await handler();

        var consumer = GetResponseHandler(response);
        consumer.Invoke(_logger);

        return response;
    }

    private static Action<ILogger> GetResponseHandler(OneOf<TResponse, IError> response) =>
        response switch
        {
            var oneOf when oneOf.Index == 0 => l => l.ValueReturned(oneOf.Value),
            var oneOf when oneOf.Index == 1 => l => l.ErrorReturned(oneOf.Value),
            var unknown => l => l.UnknownReturned(unknown),
        };
}
