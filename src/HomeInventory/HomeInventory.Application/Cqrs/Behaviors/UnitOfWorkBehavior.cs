using System.Transactions;
using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal sealed class UnitOfWorkBehavior<TRequest, TIgnored>(IScopeAccessor scopeAccessor, ILogger<UnitOfWorkBehavior<TRequest, TIgnored>> logger) : IPipelineBehavior<TRequest, Option<Error>>
    where TRequest : ICommand
{
    private static readonly string _requestName = typeof(TRequest).GetFormattedName();

    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly ILogger _logger = logger;

    public async Task<Option<Error>> Handle(TRequest request, RequestHandlerDelegate<Option<Error>> next, CancellationToken cancellationToken)
    {
        using var scope = new TransactionScope();

        var result = await next(cancellationToken);
        if (result.IsNone)
        {
            await SaveChangesAsync(scope, cancellationToken);
        }

        return result;
    }

    private async Task SaveChangesAsync(TransactionScope transactionScope, CancellationToken cancellationToken)
    {
        var unitOfWork = _scopeAccessor.GetRequiredContext<IUnitOfWork>();

        var count = await unitOfWork.SaveChangesAsync(cancellationToken);
        switch (count)
        {
            case 0:
                _logger.HandleUnitOfWorkNotSaved(_requestName);
                break;
            default:
                _logger.HandleUnitOfWorkSaved(_requestName, count);
                break;
        }

        transactionScope.Complete();
    }
}
