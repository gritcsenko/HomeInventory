using HomeInventory.Web.OpenApi;

namespace HomeInventory.Tests.Systems.Modules;

public sealed class JsonOpenApiValueConverterGivenContext(BaseTest test) : GivenContext<JsonOpenApiValueConverterGivenContext, JsonOpenApiValueConverter>(test)
{
    private readonly Variable<object> _value = new(nameof(_value));

    public JsonOpenApiValueConverterGivenContext NullValue(out IVariable<object> valueVariable) => Value(null!, out valueVariable);

    public JsonOpenApiValueConverterGivenContext DbNullValue(out IVariable<object> valueVariable) => Value(DBNull.Value, out valueVariable);

    public JsonOpenApiValueConverterGivenContext Value(object value, out IVariable<object> valueVariable)
    {
        valueVariable = _value;
        return Add(_value, value!);
    }

    protected override JsonOpenApiValueConverter CreateSut() => new();
}
