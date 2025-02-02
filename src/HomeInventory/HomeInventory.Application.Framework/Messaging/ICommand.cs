namespace HomeInventory.Application.Framework.Messaging;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "By Design, marker interface")]
public interface ICommand : IRequest<Option<Error>>
{
}
