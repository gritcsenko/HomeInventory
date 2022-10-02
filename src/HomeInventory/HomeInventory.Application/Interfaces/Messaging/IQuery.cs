using FluentResults;
using MediatR;

namespace HomeInventory.Application.Interfaces.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}

