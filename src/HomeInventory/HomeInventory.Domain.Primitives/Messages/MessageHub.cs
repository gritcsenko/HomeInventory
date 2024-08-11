using HomeInventory.Domain.Primitives.Ids;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace HomeInventory.Domain.Primitives.Messages;

public sealed class MessageHub(IIdSupplier<Ulid> supplier, TimeProvider timeProvider, IMessageObservableProvider observableProvider) : DisposableBase, IMessageHub
{
    private readonly ConcurrentDictionary<Type, object> _observables = new();
    private readonly IMessageObservableProvider _observableProvider = observableProvider;

    public IIdSupplier<Ulid> EventIdSupplier { get; } = supplier;

    public TimeProvider EventCreatedTimeProvider { get; } = timeProvider;

    public void OnNext<TMessage>(TMessage message)
        where TMessage : IMessage =>
        GetSubject<TMessage>().OnNext(message);

    public IObservable<TMessage> GetMessages<TMessage>()
        where TMessage : IMessage =>
        GetSubject<TMessage>().AsObservable();

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Dispose(_observables.Values.OfType<IDisposable>());
        }

        base.Dispose(disposing);
    }

    private ISubject<TMessage> GetSubject<TMessage>() where TMessage : IMessage
    {
        ObjectDisposedException.ThrowIf(IsDisposingOrDisposed, GetType().Name);
        return (ISubject<TMessage>)_observables.GetOrAdd(typeof(TMessage), CreateSubject<TMessage>);
    }

    private ISubject<TMessage> CreateSubject<TMessage>(Type _)
        where TMessage : IMessage =>
        _observableProvider.GetSubject<TMessage>(this);
}
