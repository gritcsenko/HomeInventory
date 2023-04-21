using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Domain.ValueObjects;

internal sealed class AmountFactory : ValueObjectFactory<Amount>, IAmountFactory
{
    private static readonly IReadOnlyDictionary<AmountUnit, Func<decimal, bool>> Validators = new Dictionary<AmountUnit, Func<decimal, bool>>
    {
        [AmountUnit.Kelvin] = x => x >= 0m,
    };

    public OneOf<Amount, IError> Create(decimal value, AmountUnit unit)
    {
        var validateFunc = Validators.GetValueOrDefault(unit, SelectValidator);
        return TryCreate(() => validateFunc(value), () => CreateValidationError((value, unit)), () => new Amount(value, unit));
    }

    private Func<decimal, bool> SelectValidator(AmountUnit unit) => unit switch
    {
        { } u when u.Measurement == MeasurementType.Count => x => x >= 0m && decimal.Truncate(x) == x,
        { } u when u.Measurement == MeasurementType.Volume => x => x >= 0m,
        { } u when u.Measurement == MeasurementType.Area => x => x >= 0m,
        { } u when u.Measurement == MeasurementType.Length => x => x >= 0m,
        { } u when u.Measurement == MeasurementType.Weight => x => x >= 0m,
        _ => x => true,
    };
}
