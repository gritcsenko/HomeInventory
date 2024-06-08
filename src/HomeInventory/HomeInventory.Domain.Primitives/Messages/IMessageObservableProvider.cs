using System.Reactive.Subjects;

namespace HomeInventory.Domain.Primitives.Messages;

public interface IMessageObservableProvider
{
    ISubject<TMessage> GetSubject<TMessage>(IMessageHub hub) where TMessage : IMessage;
}
