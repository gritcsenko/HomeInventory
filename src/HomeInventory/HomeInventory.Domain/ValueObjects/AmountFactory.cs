using DotNext;
using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Domain.ValueObjects;

internal sealed class AmountFactory : IAmountFactory
{
    public OneOf<Amount, IError> Create(decimal value, AmountUnit unit) =>
        new UnitValidator(unit)
            .Validate(value)
            .Convert(OneOf<Amount, IError>.FromT1)
            .OrInvoke(() => new Amount(value, unit));

    private readonly ref struct UnitValidator(AmountUnit unit)
    {
        private readonly AmountUnit _unit = unit;

        public Optional<IError> Validate(decimal value)
        {
            var validator = SelectValidator();
            if (!IsValid(validator, value))
            {
                return Optional.Some<IError>(GetValidationError((value, _unit)));
            }

            return Optional.None<IError>();
        }

        private static bool IsValid(Func<decimal, bool> validator, decimal value)
        {
            return validator(value);
        }

        private static ObjectValidationError<TValue> GetValidationError<TValue>(TValue value) => new ObjectValidationError<TValue>(value);

        private Func<decimal, bool> SelectValidator() => _unit switch
        {
            { } u when u == AmountUnit.Kelvin => NonNegative,
            { } u when u.Measurement == MeasurementType.Count => And(NonNegative, NonFractional),
            { } u when u.Measurement == MeasurementType.Volume => NonNegative,
            { } u when u.Measurement == MeasurementType.Area => NonNegative,
            { } u when u.Measurement == MeasurementType.Length => NonNegative,
            { } u when u.Measurement == MeasurementType.Weight => NonNegative,
            _ => AnyIsValid,
        };

        private static Func<decimal, bool> And(Func<decimal, bool> left, Func<decimal, bool> right) => x => left(x) && right(x);

        private static bool AnyIsValid(decimal _) => true;

        private static bool NonNegative(decimal value) => value >= 0;

        private static bool NonFractional(decimal value) => decimal.Truncate(value) == value;
    }
}
