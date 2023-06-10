namespace HomeInventory.Tests.Framework;

public class ValueContainer
{
    private Optional<object> _value;
    private readonly Type _type;

    public ValueContainer(Optional<object> value, Type type)
    {
        _value = value;
        _type = type;
    }

    public Optional<object> Value => _value;

    public Type Type => _type;

    public void Update(Optional<object> value) => _value = value;
}
