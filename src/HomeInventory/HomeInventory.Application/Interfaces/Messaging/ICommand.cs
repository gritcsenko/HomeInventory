using HomeInventory.Domain.Errors;
using MediatR;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Interfaces.Messaging;

public interface ICommand<TResponse> : IRequest<OneOf<TResponse, IError>>
{
}

public interface ICommand : ICommand<Success>
{
}
