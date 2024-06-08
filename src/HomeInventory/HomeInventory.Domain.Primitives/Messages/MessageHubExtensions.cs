using HomeInventory.Domain.Primitives.Errors;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;

namespace HomeInventory.Domain.Primitives.Messages;

public static class MessageHubExtensions
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "Implementation details with generics")]
    private static readonly MethodInfo _request = typeof(MessageHubExtensions)
        .GetMethod(nameof(InternalRequestAsync), BindingFlags.NonPublic | BindingFlags.Static)!
        .GetGenericMethodDefinition();

    public static Task<OneOf<TResponse, IError>> RequestAsync<TResponse>(this IMessageHub hub, IRequestMessage<TResponse> request, CancellationToken cancellationToken = default) =>
        (Task<OneOf<TResponse, IError>>)_request.MakeGenericMethod(request.GetType(), typeof(TResponse)).Invoke(null, [hub, request, cancellationToken])!;

    public static ResposeMessage<TRequest, TResponse> CreateResponse<TRequest, TResponse>(this IMessageHub hub, TRequest request, OneOf<TResponse, IError> response)
        where TRequest : IRequestMessage<TResponse> =>
        hub.CreateMessage((id, on) => new ResposeMessage<TRequest, TResponse>(id, on, request, response));

    public static TMessage CreateMessage<TMessage>(this IMessageHub hub, Func<Cuid, DateTimeOffset, TMessage> createFunc)
        where TMessage : IMessage =>
        createFunc(hub.EventIdSupplier.Invoke(), hub.EventCreatedTimeProvider.GetUtcNow());

    private static Task<OneOf<TResponse, IError>> InternalRequestAsync<TRequest, TResponse>(this IMessageHub hub, TRequest request, CancellationToken cancellationToken)
        where TRequest : class, IRequestMessage<TResponse>
    {
        var task = hub.GetMessages<ResposeMessage<TRequest, TResponse>>()
            .Where(e => ReferenceEquals(e.Request, request))
            .Select(e => e.Result)
            .Take(1)
            .ToTask(cancellationToken);

        var @event = request.ToCancellable<TRequest, TResponse>(cancellationToken);
        hub.OnNext(@event);

        return task;
    }

    private static CancellableRequest<TRequest, TResponse> ToCancellable<TRequest, TResponse>(this TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequestMessage<TResponse> =>
        new(request, cancellationToken);
}
