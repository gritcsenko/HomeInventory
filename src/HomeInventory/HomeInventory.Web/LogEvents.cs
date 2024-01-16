using Microsoft.Extensions.Logging;

namespace HomeInventory.Web;

internal static partial class LogEvents
{
    [LoggerMessage(Level = LogLevel.Information, Message = "New {CorrelationId} was generated")]
    public static partial void CorrelationIdGenerated(this ILogger logger, string correlationId);

    [LoggerMessage(Level = LogLevel.Information, Message = "{CorrelationId} was returned to the caller")]
    public static partial void CorrelationIdReturned(this ILogger logger, string correlationId);
}
