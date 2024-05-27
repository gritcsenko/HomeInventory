using HomeInventory.Domain.Primitives.Errors;
using OneOf.Types;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace HomeInventory.Domain.Primitives.Events;

public static class EventHubExtensions
{
    public static Task<OneOf<Success, IError>> RequestAsync<TRequest>(this IEventHub hub, TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class, IRequestEvent =>
        hub.InternalRequestAsync<TRequest, IResposeEvent<TRequest>, Success>(request, cancellationToken);

    public static Task<OneOf<TResult, IError>> RequestAsync<TRequest, TResult>(this IEventHub hub, TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class, IRequestEvent<TResult> =>
        hub.InternalRequestAsync<TRequest, IResposeEvent<TRequest, TResult>, TResult>(request, cancellationToken);

    private static Task<OneOf<TResult, IError>> InternalRequestAsync<TRequest, TResponse, TResult>(this IEventHub hub, TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class, IEvent
        where TResponse : IRequestResultEvent<TRequest, TResult>
    {
        var task = hub.GetEvents<TResponse>()
            .Where(e => ReferenceEquals(e.Request, request))
            .Select(e => e.Result)
            .ToTask(cancellationToken);

        hub.Notify(new CancellableRequestEvent<TRequest>(request.Id, request, cancellationToken));

        return task;
    }
}
