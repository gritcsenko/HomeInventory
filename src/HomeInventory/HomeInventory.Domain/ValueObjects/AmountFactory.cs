using DotNext;
using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Domain.ValueObjects;

internal sealed class AmountFactory : IAmountFactory
{
    public OneOf<Amount, IError> Create(decimal value, AmountUnit unit) =>
        new Validator(unit)
            .Validate(value)
            .Convert(OneOf<Amount, IError>.FromT1)
            .OrInvoke(() => new Amount(value, unit));

    private class Validator
    {
        private readonly AmountUnit _unit;

        public Validator(AmountUnit unit) => _unit = unit;

        public Optional<IError> Validate(decimal value)
        {
            var func = SelectValidator();
            if (func(value))
            {
                return Optional.None<IError>();
            }

            return Optional.Some(GetValidationError((value, _unit)));
        }

        private static IError GetValidationError<TValue>(TValue value) => new ObjectValidationError<TValue>(value);

        private Func<decimal, bool> SelectValidator() => _unit switch
        {
            { } u when u == AmountUnit.Kelvin => x => x >= 0m,
            { } u when u.Measurement == MeasurementType.Count => x => x >= 0m && decimal.Truncate(x) == x,
            { } u when u.Measurement == MeasurementType.Volume => x => x >= 0m,
            { } u when u.Measurement == MeasurementType.Area => x => x >= 0m,
            { } u when u.Measurement == MeasurementType.Length => x => x >= 0m,
            { } u when u.Measurement == MeasurementType.Weight => x => x >= 0m,
            _ => x => true,
        };
    }
}
