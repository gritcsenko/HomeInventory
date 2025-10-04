using System.Diagnostics.CodeAnalysis;

namespace HomeInventory.Application.Framework.Messaging;

[SuppressMessage("Design", "CA1040:Avoid empty interfaces", Justification = "By Design, marker interface")]
public interface IRequest<[SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "Marker parameter")] TResult>
{
}
