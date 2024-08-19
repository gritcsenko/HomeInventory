using HomeInventory.Web.OpenApi;
using System.Runtime.CompilerServices;

namespace HomeInventory.Tests.Systems.Modules;

public sealed class JsonOpenApiValueConverterGivenContext(BaseTest test) : GivenContext<JsonOpenApiValueConverterGivenContext, JsonOpenApiValueConverter>(test)
{
    private readonly Variable<object> _value = new(nameof(_value));

    public JsonOpenApiValueConverterGivenContext NullValue(out IVariable<object> valueVariable, [CallerArgumentExpression(nameof(valueVariable))] string? name = null) => Value(out valueVariable, default!, name: name);

    public JsonOpenApiValueConverterGivenContext DbNullValue(out IVariable<object> valueVariable, [CallerArgumentExpression(nameof(valueVariable))] string? name = null) => Value(out valueVariable, DBNull.Value, name: name);

    public JsonOpenApiValueConverterGivenContext Value(out IVariable<object> valueVariable, object value, [CallerArgumentExpression(nameof(valueVariable))] string? name = null) => New(out valueVariable, () => value, name: name);

    protected override JsonOpenApiValueConverter CreateSut() => new();

    private sealed class NullValueObject
    {
        public override bool Equals(object? obj) => obj is NullValueObject || obj is null;

        public override int GetHashCode() => 0;
    }
}
