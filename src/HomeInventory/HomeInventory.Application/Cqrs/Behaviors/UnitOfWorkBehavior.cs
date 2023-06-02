using System.Transactions;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Cqrs.Behaviors;

internal class UnitOfWorkBehavior<TRequest, TIgnored> : IPipelineBehavior<TRequest, OneOf<Success, IError>>
     where TRequest : ICommand
{
    private static readonly string _requestName = typeof(TRequest).GetFormattedName();

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public UnitOfWorkBehavior(IUnitOfWork unitOfWork, ILogger<UnitOfWorkBehavior<TRequest, TIgnored>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<OneOf<Success, IError>> Handle(TRequest request, RequestHandlerDelegate<OneOf<Success, IError>> next, CancellationToken cancellationToken)
    {
        using var scope = new TransactionScope();

        var result = await next();

        await HandleResultAsync(result, cancellationToken);

        scope.Complete();

        return result;
    }

    private async Task HandleResultAsync(OneOf<Success, IError> result, CancellationToken cancellationToken)
    {
        if (result.IsT1)
        {
            return;
        }

        var count = await _unitOfWork.SaveChangesAsync(cancellationToken);
        switch (count)
        {
            case 0:
                _logger.LogWarning("{Request} was attempted to save changes and saved nothing", _requestName);
                break;
            default:
                _logger.LogInformation("{Request} was attempted to save changes and saved {count}", _requestName, count);
                break;
        }
    }
}
