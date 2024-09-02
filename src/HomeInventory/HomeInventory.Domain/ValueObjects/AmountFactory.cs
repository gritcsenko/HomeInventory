namespace HomeInventory.Domain.ValueObjects;

internal sealed class AmountFactory : IAmountFactory
{
    public Validation<Error, Amount> Create(decimal value, AmountUnit unit) =>
        new UnitValidator(unit)
            .Validate(value)
            .Bind<Amount>(t => new Amount(t.Value, t.Unit));

    private readonly ref struct UnitValidator(AmountUnit unit)
    {
        private readonly AmountUnit _unit = unit;

        public Validation<Error, (decimal Value, AmountUnit Unit)> Validate(decimal value)
        {
            var validator = SelectValidator();
            return validator(value)
                ? (value, _unit)
                : new ValidationError("Bad amount value", (value, _unit));
        }

        private Func<decimal, bool> SelectValidator() => _unit switch
        {
            { } u when u == AmountUnit.Kelvin => NonNegative,
            { } u when u.Measurement == MeasurementType.Count => NonNegative.And(NonFractional),
            { } u when u.Measurement == MeasurementType.Volume => NonNegative,
            { } u when u.Measurement == MeasurementType.Area => NonNegative,
            { } u when u.Measurement == MeasurementType.Length => NonNegative,
            { } u when u.Measurement == MeasurementType.Weight => NonNegative,
            _ => AnyIsValid,
        };

        private static Func<decimal, bool> AnyIsValid { get; } = static _ => true;

        private static Func<decimal, bool> NonNegative { get; } = static value => value >= 0;

        private static Func<decimal, bool> NonFractional { get; } = static value => decimal.Truncate(value) == value;
    }
}

file static class Extensions
{
    public static Func<decimal, bool> And(this Func<decimal, bool> left, Func<decimal, bool> right) => x => left(x) && right(x);
}
