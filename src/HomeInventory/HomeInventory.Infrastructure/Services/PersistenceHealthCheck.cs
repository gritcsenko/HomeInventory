using HomeInventory.Application;
using HomeInventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Services;

internal class PersistenceHealthCheck : BaseHealthCheck
{
    private readonly DatabaseContext _context;

    public PersistenceHealthCheck(DatabaseContext context) => _context = context;

    protected override async ValueTask<bool> IsHealthyAsync(CancellationToken cancellationToken) =>
        await _context.Database.CanConnectAsync(cancellationToken)
        && await CheckPendingMigrationsAsync(cancellationToken);

    private async Task<bool> CheckPendingMigrationsAsync(CancellationToken cancellationToken) =>
        _context.Database.IsRelational()
        && !(await _context.Database.GetPendingMigrationsAsync(cancellationToken)).Any();
}
