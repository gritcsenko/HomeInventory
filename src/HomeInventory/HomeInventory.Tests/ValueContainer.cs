using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests;

public class ValueContainer
{
    private Option<object> _value;
    private readonly Type _type;

    public ValueContainer(Option<object> value, Type type)
    {
        _value = value;
        _type = type;
    }

    public Option<object> Value => _value;

    public Type Type => _type;

    public void Update(Option<object> value) => _value = value;
}
