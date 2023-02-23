using HomeInventory.Domain.Errors;
using OneOf;

namespace HomeInventory.Domain.Primitives;

public interface IValueObjectFactory<TObject, TValue>
{
    OneOf<TObject, IError> CreateFrom(TValue id);
}
