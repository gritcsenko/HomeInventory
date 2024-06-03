using System.Transactions;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal sealed class UnitOfWorkRequestBehavior<TRequest>(IScopeAccessor scopeAccessor, ILogger<UnitOfWorkRequestBehavior<TRequest>> logger) : IRequestPipelineBehavior<TRequest, Success>
    where TRequest : IRequestMessage<Success>
{
    private static readonly string _requestName = typeof(TRequest).GetFormattedName();

    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly ILogger _logger = logger;

    public async Task<OneOf<Success, IError>> OnRequest(IMessageHub hub, TRequest request, Func<Task<OneOf<Success, IError>>> handler, CancellationToken cancellationToken = default)
    {
        using var scope = new TransactionScope();
        return await handler()
            .OnSuccessAsync(() => SaveChangesAsync(scope, cancellationToken));
    }

    private async Task SaveChangesAsync(TransactionScope transactionScope, CancellationToken cancellationToken)
    {
        var unitOfWork = _scopeAccessor.TryGet<IUnitOfWork>().OrThrow<InvalidOperationException>();
        var count = await unitOfWork.SaveChangesAsync(cancellationToken);
        transactionScope.Complete();

        switch (count)
        {
            case 0:
                _logger.HandleUnitOfWorkNotSaved(_requestName);
                break;
            default:
                _logger.HandleUnitOfWorkSaved(_requestName, count);
                break;
        }

    }
}
