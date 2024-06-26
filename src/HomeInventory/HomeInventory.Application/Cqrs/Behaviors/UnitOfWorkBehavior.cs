﻿using System.Transactions;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal sealed class UnitOfWorkBehavior<TRequest, TIgnored>(IScopeAccessor scopeAccessor, ILogger<UnitOfWorkBehavior<TRequest, TIgnored>> logger) : IPipelineBehavior<TRequest, OneOf<Success, IError>>
    where TRequest : ICommand
{
    private static readonly string _requestName = typeof(TRequest).GetFormattedName();

    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly ILogger _logger = logger;

    public async Task<OneOf<Success, IError>> Handle(TRequest request, RequestHandlerDelegate<OneOf<Success, IError>> next, CancellationToken cancellationToken)
    {
        using var scope = new TransactionScope();

        var result = await next();
        if (result.IsT0)
        {
            await SaveChangesAsync(scope, cancellationToken);
        }

        return result;
    }

    private async Task SaveChangesAsync(TransactionScope transactionScope, CancellationToken cancellationToken)
    {
        var serviceScope = _scopeAccessor.GetScope<IUnitOfWork>();
        var unitOfWork = serviceScope.Get().OrThrow<InvalidOperationException>();
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
