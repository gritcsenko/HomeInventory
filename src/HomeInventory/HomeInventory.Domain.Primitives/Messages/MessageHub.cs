using System.Collections.Concurrent;
using System.Reactive.Linq;

namespace HomeInventory.Domain.Primitives.Messages;

public sealed class MessageHub(ISupplier<Cuid> supplier, TimeProvider timeProvider, IMessageObservableProvider observableProvider) : Disposable, IMessageHub
{
    private readonly ConcurrentDictionary<Type, object> _observables = new();
    private readonly IMessageObservableProvider _observableProvider = observableProvider;

    public ISupplier<Cuid> EventIdSupplier { get; } = supplier;

    public TimeProvider EventCreatedTimeProvider { get; } = timeProvider;

    public void Inject<TMessage>(IObservable<TMessage> messages)
        where TMessage : IMessage
    {
        EnsureNotDisposing();
        _observables.AddOrUpdate(typeof(TMessage), CreateObservable, UpdateObservable, messages);
    }

    public IObservable<TMessage> GetMessages<TMessage>()
        where TMessage : IMessage
    {
        EnsureNotDisposing();
        return (IObservable<TMessage>)_observables.GetOrAdd(typeof(TMessage), CreateObservable<TMessage>);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Dispose(_observables.Values.OfType<IDisposable>());
        }

        base.Dispose(disposing);
    }

    private IObservable<TMessage> CreateObservable<TMessage>(Type type, IObservable<TMessage> messages)
        where TMessage : IMessage =>
        UpdateObservable(type, CreateObservable<TMessage>(type), messages);

    private IObservable<TMessage> CreateObservable<TMessage>(Type _)
        where TMessage : IMessage =>
        _observableProvider.GetObservable<TMessage>(this);

    private static IObservable<TMessage> UpdateObservable<TMessage>(Type _, object existingObj, IObservable<TMessage> messages)
        where TMessage : IMessage
    {
        var existing = (IObservable<TMessage>)existingObj;
        return existing.Merge(messages);
    }

    private void EnsureNotDisposing() => ObjectDisposedException.ThrowIf(IsDisposingOrDisposed, nameof(MessageHub));
}
