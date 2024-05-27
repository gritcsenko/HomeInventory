using HomeInventory.Domain.Primitives.Errors;
using OneOf.Types;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace HomeInventory.Domain.Primitives.Events;

public static class EventHubExtensions
{
    public static Task<OneOf<Success, IError>> RequestAsync<TRequest>(this IEventHub hub, TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class, IRequestEvent =>
        hub.InternalRequestAsync<TRequest, IResposevent<TRequest>, Success>(request, cancellationToken);

    public static Task<OneOf<TResult, IError>> RequestAsync<TRequest, TResult>(this IEventHub hub, TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class, IRequestEvent<TResult> =>
        hub.InternalRequestAsync<TRequest, IResposeEvent<TRequest, TResult>, TResult>(request, cancellationToken);

    private static async Task<OneOf<TResult, IError>> InternalRequestAsync<TRequest, TResponse, TResult>(this IEventHub hub, TRequest request, CancellationToken cancellationToken = default)
        where TRequest : class, IEvent
        where TResponse : IRequestResultEvent<TRequest, TResult>
    {
        using var subj = new Subject<TResponse>();
        using var subscription = hub.Subscribe(subj);
        hub.Notify(request);
        var responseEvent = await subj.Where(e => ReferenceEquals(e.Request, request)).ToTask(cancellationToken);
        return responseEvent.Result;
    }
}
