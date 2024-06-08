using Microsoft.Extensions.DependencyInjection;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Reflection;

namespace HomeInventory.Domain.Primitives.Messages;

public sealed class MessageObservableProvider(IServiceProvider serviceProvider) : IMessageObservableProvider, IDisposable
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "Implementation details with generics")]
    private static readonly MethodInfo _tryLinkRequests = typeof(MessageObservableProvider)
        .GetMethod(nameof(TryLinkRequests), 2, BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(IMessageHub)], null)!
        .GetGenericMethodDefinition();
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly CompositeDisposable _disposables = [];
    private bool _isDisposed;

    public ISubject<TMessage> GetSubject<TMessage>(IMessageHub hub)
        where TMessage : IMessage
    {
        TryLinkMessages<TMessage>(hub);
        TryLinkRequests<TMessage>(hub);

        return new Subject<TMessage>();
    }

    private void TryLinkMessages<TMessage>(IMessageHub hub)
        where TMessage : IMessage
    {
        if (_serviceProvider.GetService<IMessageHandler<TMessage>>() is null)
        {
            return;
        }

        var link = LinkRequestsFor<MessageHandlerAdapter<TMessage>>(hub);
        _disposables.Add(link);
    }

    private void TryLinkRequests<TMessage>(IMessageHub hub)
        where TMessage : IMessage
    {
        var messageType = typeof(TMessage);
        if (!messageType.IsGenericType || messageType.GetGenericTypeDefinition() != typeof(CancellableRequest<,>)) // CancellableRequest<TRequest, TResponse>
        {
            return;
        }

        var requestType = messageType.GetGenericArguments()[0];
        var responseType = messageType.GetGenericArguments()[1];
        var tryLink = _tryLinkRequests.MakeGenericMethod(requestType, responseType);
        tryLink.Invoke(this, [hub]);
    }

    private void TryLinkRequests<TRequest, TResponse>(IMessageHub hub)
        where TRequest : IRequestMessage<TResponse>
    {
        if (_serviceProvider.GetService<IRequestHandler<TRequest, TResponse>>() is null)
        {
            return;
        }

        var link = LinkRequestsFor<RequestHandlerAdapter<TRequest, TResponse>>(hub);
        _disposables.Add(link);
    }

    private IDisposable LinkRequestsFor<TAdapter>(IMessageHub hub)
        where TAdapter : IMessageHandlerAdapter
    {
        var adapter = _serviceProvider.GetRequiredService<TAdapter>();
        return adapter.Subscribe(hub);
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _disposables.Dispose();
            }

            _isDisposed = true;
        }
    }
}
