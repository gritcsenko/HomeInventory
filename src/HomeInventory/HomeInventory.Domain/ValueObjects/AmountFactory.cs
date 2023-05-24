using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Domain.ValueObjects;

internal sealed class AmountFactory : IAmountFactory
{
    public OneOf<Amount, IError> Create(decimal value, AmountUnit unit) =>
        TryCreate((value, unit), x => Validator.Validate(x.value, x.unit), x => new Amount(x.value, x.unit));

    private static OneOf<Amount, IError> TryCreate<TValue>(TValue value, Func<TValue, bool> isValidFunc, Func<TValue, Amount> createFunc)
    {
        if (isValidFunc(value))
        {
            return createFunc(value);
        }

        return new ObjectValidationError<TValue>(value);
    }

    private class Validator
    {
        private static readonly IReadOnlyDictionary<AmountUnit, Func<decimal, bool>> _validators = new Dictionary<AmountUnit, Func<decimal, bool>>
        {
            [AmountUnit.Kelvin] = x => x >= 0m,
        };

        public static bool Validate(decimal value, AmountUnit unit)
        {
            var validateFunc = _validators.GetValueOrDefault(unit, SelectValidator);
            return validateFunc(value);
        }

        private static Func<decimal, bool> SelectValidator(AmountUnit unit) => unit switch
        {
            { } u when u.Measurement == MeasurementType.Count => x => x >= 0m && decimal.Truncate(x) == x,
            { } u when u.Measurement == MeasurementType.Volume => x => x >= 0m,
            { } u when u.Measurement == MeasurementType.Area => x => x >= 0m,
            { } u when u.Measurement == MeasurementType.Length => x => x >= 0m,
            { } u when u.Measurement == MeasurementType.Weight => x => x >= 0m,
            _ => x => true,
        };
    }
}
