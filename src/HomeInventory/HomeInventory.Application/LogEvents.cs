namespace HomeInventory.Application;

internal static partial class LogEvents
{
    internal static readonly EventId _handleUnitOfWork = new(1, "Handle UnitOfWork result");
    internal static readonly EventId _sendingRequest = new(2, "Sending request");
    internal static readonly EventId _handleResponse = new(3, "Handle response");

    private static readonly Func<ILogger, string, string, IDisposable?> _loggingBehaviorScopeOld = LoggerMessage.DefineScope<string, string>("Got {Request} and will return {Response}");
    private static readonly Func<ILogger, string, IDisposable?> _loggingBehaviorScope = LoggerMessage.DefineScope<string>("Got {Request}");

    private static readonly Action<ILogger, string, Exception?> _loggingBehaviorSending = LoggerMessage.Define<string>(LogLevel.Information, _sendingRequest, "Sending {Request}");

    private static readonly Action<ILogger, string, Type, Exception?> _loggingBehaviorUnknown = LoggerMessage.Define<string, Type>(LogLevel.Error, _handleResponse, "{Response} of type {Type} was returned");

    [LoggerMessage(Level = LogLevel.Warning, Message = "{Request} was attempted to save changes and saved nothing")]
    public static partial void HandleUnitOfWorkNotSaved(this ILogger logger, string request);

    [LoggerMessage(Level = LogLevel.Information, Message = "{Request} was attempted to save changes and saved {Count}")]
    public static partial void HandleUnitOfWorkSaved(this ILogger logger, string request, int count);

    public static IDisposable? LoggingBehaviorScope(this ILogger logger, string requestName, string responseName) =>
        _loggingBehaviorScopeOld(logger, requestName, responseName);

    public static IDisposable? LoggingBehaviorScope(this ILogger logger, string requestName) =>
        _loggingBehaviorScope(logger, requestName);

    public static void SendingRequest<TRequest>(this ILogger logger, TRequest request)
        where TRequest : notnull =>
        _loggingBehaviorSending(logger, request.ToString() ?? string.Empty, null);

    [LoggerMessage(Level = LogLevel.Information, Message = "{Value} was returned")]
    public static partial void ValueReturned(this ILogger logger, object value);

    [LoggerMessage(Level = LogLevel.Warning, Message = "{Error} was returned")]
    public static partial void ErrorReturned(this ILogger logger, object error);

    public static void UnknownReturned<TResponse>(this ILogger logger, TResponse? response) =>
        _loggingBehaviorUnknown(logger, response?.ToString() ?? string.Empty, typeof(TResponse), null);
}
