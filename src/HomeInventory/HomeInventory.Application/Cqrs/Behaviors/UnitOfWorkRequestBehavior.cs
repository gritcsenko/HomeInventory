using System.Transactions;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal sealed class UnitOfWorkRequestBehavior<TRequest, TResponse>(IScopeAccessor scopeAccessor, ILogger<UnitOfWorkRequestBehavior<TRequest, TResponse>> logger) : IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequestMessage<TResponse>
{
    private static readonly string _requestName = typeof(TRequest).GetFormattedName();

    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly ILogger _logger = logger;

    public async Task<TResponse> OnRequestAsync(IRequestContext<TRequest> context, Func<IRequestContext<TRequest>, Task<TResponse>> handler)
    {
        using var scope = new TransactionScope();

        var response = await handler(context);
        switch (response)
        {
            case Option<Error> option when option.IsNone:
            case IQueryResult result when result.IsSuccess:
                await SaveChangesAsync(scope, context.RequestAborted);
                break;
        }

        return response;
    }

    private async Task SaveChangesAsync(TransactionScope transactionScope, CancellationToken cancellationToken)
    {
        var unitOfWork = _scopeAccessor.GetRequiredContext<IUnitOfWork>();
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
