namespace HomeInventory.Domain.Primitives.Messages;

public interface IMessageHandlerAdapter
{
    IDisposable Subscribe(IMessageHub hub);
}
