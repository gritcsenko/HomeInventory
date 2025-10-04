using Microsoft.Extensions.Logging;

namespace HomeInventory.Application.UserManagement;

internal static class LogEvents
{
    internal static readonly EventId _handleUnitOfWork = new(1, "Handle UnitOfWork result");

    private static readonly Action<ILogger, string, int, Exception?> _unitOfWorkSaved = LoggerMessage.Define<string, int>(LogLevel.Information, _handleUnitOfWork, "{Request} was attempted to save changes and saved {Count}");
    private static readonly Action<ILogger, string, Exception?> _unitOfWorkNotSaved = LoggerMessage.Define<string>(LogLevel.Warning, _handleUnitOfWork, "{Request} was attempted to save changes and saved nothing");

    public static void HandleUnitOfWorkNotSaved<TRequest>(this ILogger logger) =>
        _unitOfWorkNotSaved(logger, typeof(TRequest).GetFormattedName(), null);

    public static void HandleUnitOfWorkSaved<TRequest>(this ILogger logger, int count) =>
        _unitOfWorkSaved(logger, typeof(TRequest).GetFormattedName(), count, null);
}
