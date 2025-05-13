using System.Diagnostics.CodeAnalysis;

namespace HomeInventory.Application.Framework.Messaging;

[SuppressMessage("Design", "CA1040:Avoid empty interfaces")]
public interface IBaseRequest
{
    
}

[SuppressMessage("Design", "CA1040:Avoid empty interfaces")]
public interface IRequest : IBaseRequest
{
}

[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "By Design, marker interface")]
[SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed")]
[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public interface IRequest<TResponse> : IRequest
{
}

[SuppressMessage("Design", "CA1040:Avoid empty interfaces")]
public interface INotification
{
    
}

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken = default);
}

public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, Func<CancellationToken, Task<TResponse>> next, CancellationToken cancellationToken = default);
}

public interface ISender
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}

public interface IPublisher
{
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification;
}

public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken = default);
}

public interface IMediator : ISender, IPublisher
{
}