using FluentResults;
using MediatR;
using OneOf;

namespace HomeInventory.Application.Interfaces.Messaging;

public interface ICommand<TResponse> : IRequest<OneOf<TResponse, IError>>
{
}

public interface ICommand : ICommand<Success>
{
}
