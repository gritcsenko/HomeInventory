using FluentResults;
using MediatR;

namespace HomeInventory.Application.Interfaces.Messaging;

public interface ICommand<TResponse> : IRequest<IResult<TResponse>>
{
}

public interface ICommand : ICommand<Success>
{
}
