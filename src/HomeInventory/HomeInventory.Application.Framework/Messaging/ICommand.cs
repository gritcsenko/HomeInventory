using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Interfaces.Messaging;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "By Design, marker interface")]
public interface ICommand : IRequest<OneOf<Success, IError>>
{
}
