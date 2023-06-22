namespace HomeInventory.Application;

internal static class LogEvents
{
    internal static readonly EventId _handleUnitOfWork = new(1, "Handle UnitOfWork result");
    internal static readonly EventId _sendingRequest = new(2, "Sending request");
    internal static readonly EventId _handleResponse = new(3, "Handle response");

    private static readonly Action<ILogger, string, Exception?> _handleUnitOfWorkNotSaved = LoggerMessage.Define<string>(LogLevel.Warning, _handleUnitOfWork, "{Request} was attempted to save changes and saved nothing");

    private static readonly Action<ILogger, string, int, Exception?> _handleUnitOfWorkSaved = LoggerMessage.Define<string, int>(LogLevel.Information, _handleUnitOfWork, "{Request} was attempted to save changes and saved {Count}");

    private static readonly Func<ILogger, string, string, IDisposable?> _loggingBehaviorScope = LoggerMessage.DefineScope<string, string>("Got {Request} and will return {Response}");

    private static readonly Action<ILogger, string, Exception?> _loggingBehaviorSending = LoggerMessage.Define<string>(LogLevel.Information, _sendingRequest, "Sending {Request}");

    private static readonly Action<ILogger, string, Exception?> _loggingBehaviorValue = LoggerMessage.Define<string>(LogLevel.Information, _handleResponse, "{Value} was returned");

    private static readonly Action<ILogger, string, Exception?> _loggingBehaviorError = LoggerMessage.Define<string>(LogLevel.Warning, _handleResponse, "{Error} was returned");

    public static void HandleUnitOfWorkNotSaved(this ILogger logger, string requestName) =>
        _handleUnitOfWorkNotSaved(logger, requestName, null);

    public static void HandleUnitOfWorkSaved(this ILogger logger, string requestName, int count) =>
        _handleUnitOfWorkSaved(logger, requestName, count, null);

    public static IDisposable? LoggingBehaviorScope(this ILogger logger, string requestName, string responseName) =>
        _loggingBehaviorScope(logger, requestName, responseName);

    public static void SendingRequest<TRequest>(this ILogger logger, TRequest request)
        where TRequest : notnull =>
        _loggingBehaviorSending(logger, request.ToString() ?? string.Empty, null);

    public static void ValueReturned(this ILogger logger, object value) =>
        _loggingBehaviorValue(logger, value.ToString() ?? string.Empty, null);

    public static void ErrorReturned(this ILogger logger, object error) =>
        _loggingBehaviorError(logger, error.ToString() ?? string.Empty, null);
}
