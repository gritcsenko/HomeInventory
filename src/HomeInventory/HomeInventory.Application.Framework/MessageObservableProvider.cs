using Microsoft.Extensions.DependencyInjection;
using OneOf.Types;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;

namespace HomeInventory.Domain.Primitives.Messages;

public sealed class MessageObservableProvider(IServiceProvider serviceProvider) : IMessageObservableProvider, IDisposable
{
    private static readonly MethodInfo _tryLinkRequests = typeof(MessageObservableProvider)
        .GetRuntimeMethods()
        .First(m => m.Name == nameof(InternalTryLinkRequests))
        .GetGenericMethodDefinition();
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly CompositeDisposable _disposables = [];
    private bool _isDisposed;

    public ISubject<TMessage> GetSubject<TMessage>(IMessageHub hub)
        where TMessage : IMessage
    {
        var subject = new Subject<TMessage>();

        TryLinkMessages(hub, subject);
        TryLinkRequests(hub, subject);

        return subject;
    }

    private void TryLinkMessages<TMessage>(IMessageHub hub, IObservable<TMessage> observable)
        where TMessage : IMessage
    {
        if (_serviceProvider.GetService<IMessageHandler<TMessage>>() is null)
        {
            return;
        }

        var link = LinkRequestsFor<MessageHandlerAdapter<TMessage>, TMessage>(hub, observable);
        _disposables.Add(link);
    }

    private void TryLinkRequests<TMessage>(IMessageHub hub, IObservable<TMessage> observable)
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
        tryLink.Invoke(this, [hub, observable]);
    }

    private void InternalTryLinkRequests<TRequest, TResponse>(IMessageHub hub, IObservable<CancellableRequest<TRequest, TResponse>> observable)
        where TRequest : IRequestMessage<TResponse>
    {
        if (_serviceProvider.GetService<IRequestHandler<TRequest, TResponse>>() is null)
        {
            return;
        }

        var link = LinkRequestsFor<RequestHandlerAdapter<TRequest, TResponse>, CancellableRequest<TRequest, TResponse>>(hub, observable);
        _disposables.Add(link);
    }

    private IDisposable LinkRequestsFor<TAdapter, TMessage>(IMessageHub hub, IObservable<TMessage> observable)
        where TAdapter : IMessageHandlerAdapter<TMessage>
        where TMessage : IMessage
    {
        var adapter = _serviceProvider.GetRequiredService<TAdapter>();
        return adapter.Subscribe(observable.Select(m => (hub, m)));
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
