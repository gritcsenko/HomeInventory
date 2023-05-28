using DotNext;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Mapping;

public class StringObjectConverter<TObject> : ObjectConverter<TObject, string>
    where TObject : notnull, ValueObject<TObject>, IBuildable<TObject, ValueObjectBuilder<TObject, string>>
{
    public StringObjectConverter()
        : base(x => !string.IsNullOrEmpty(x))
    {
    }
}
