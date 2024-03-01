namespace HomeInventory.Tests.Framework;

public class ValueContainer(object value, Type type)
{
    public object Value { get; private set; } = value;

    public Type Type { get; } = type;

    public void Update(object value) => Value = value;
}
