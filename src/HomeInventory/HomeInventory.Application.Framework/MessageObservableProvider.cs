using Microsoft.Extensions.DependencyInjection;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
    private bool _disposedValue;

    public IObservable<TMessage> GetObservable<TMessage>(IMessageHub hub)
        where TMessage : IMessage
    {
        TryLinkMessages<TMessage>(hub);
        TryLinkRequests<TMessage>(hub);

        return Observable.Never<TMessage>();
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
        var template = typeof(IRequestMessage<>);
        var requestType = typeof(TMessage);
        var interfaces = requestType
            .GetInterfaces()
            .Where(i => i.IsGenericType).Where(i => i.GetGenericTypeDefinition() == template);
        foreach (var iface in interfaces)
        {
            var responseType = iface.GetGenericArguments()[0];
            var tryLink = _tryLinkRequests.MakeGenericMethod(requestType, responseType);
            tryLink.Invoke(this, [hub]);
        }
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
        if (!_disposedValue)
        {
            if (disposing)
            {
                _disposables.Dispose();
            }

            _disposedValue = true;
        }
    }
}
