using Microsoft.Extensions.Logging;

namespace HomeInventory.Web;

internal static class LogEvents
{
    private static readonly EventId _newCorrelationId = new(4, "CorrelationId generated");

    private static readonly EventId _correlationIdReturned = new(4, "CorrelationId returned");

    private static readonly Action<ILogger, string, Exception?> _correlationIdGenerated = LoggerMessage.Define<string>(LogLevel.Information, _newCorrelationId, "New {CorrelationId} was generated");

    private static readonly Action<ILogger, string, Exception?> _correlationIdReturnedToCaller = LoggerMessage.Define<string>(LogLevel.Information, _correlationIdReturned, "{CorrelationId} was returned to the caller");

    public static void CorrelationIdGenerated(this ILogger logger, object value) =>
        _correlationIdGenerated(logger, value.ToString() ?? string.Empty, null);

    public static void CorrelationIdReturned(this ILogger logger, object value) =>
        _correlationIdReturnedToCaller(logger, value.ToString() ?? string.Empty, null);
}
