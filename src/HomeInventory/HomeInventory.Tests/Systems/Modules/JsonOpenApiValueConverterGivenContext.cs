using HomeInventory.Web.OpenApi;

namespace HomeInventory.Tests.Systems.Modules;

public sealed class JsonOpenApiValueConverterGivenContext(BaseTest test) : GivenContext<JsonOpenApiValueConverterGivenContext, JsonOpenApiValueConverter>(test)
{
    private readonly Variable<object> _value = new(nameof(_value));

    public JsonOpenApiValueConverterGivenContext NullValue(out IVariable<object> valueVariable) => New(out valueVariable, () => null!);

    public JsonOpenApiValueConverterGivenContext DbNullValue(out IVariable<object> valueVariable) => New(out valueVariable, () => DBNull.Value);

    public JsonOpenApiValueConverterGivenContext Value(out IVariable<object> valueVariable, object value) => New(out valueVariable, () => value);

    protected override JsonOpenApiValueConverter CreateSut() => new();
}
