using Ardalis.SmartEnum;
using ErrorOr;

namespace HomeInventory.Domain.ValueObjects;

public class Amount : ValueObject<Amount>
{
    private readonly decimal _value;
    private readonly AmountUnit _unit;

    internal Amount(decimal value, AmountUnit unit)
    {
        _value = value;
        _unit = unit;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _value;
        yield return _unit;
    }
}

public interface IAmountFactory
{
    ErrorOr<Amount> Pieces(int value);
}

public class AmountFactory : ValueObjectFactory<Amount>, IAmountFactory
{
    private static readonly Validator _piecesValidator = new(AmountUnit.Pieces, x => x >= 0m);

    public ErrorOr<Amount> Pieces(int value) => Create(value, _piecesValidator);

    private ErrorOr<Amount> Create(decimal value, Validator validator) => validator.IsValid(value) ? new Amount(value, validator.Unit) : GetValidationError();

    private class Validator
    {
        private readonly Func<decimal, bool> _validatorFunc;

        public Validator(AmountUnit unit, Func<decimal, bool> validatorFunc) => (Unit, _validatorFunc) = (unit, validatorFunc);

        public AmountUnit Unit { get; }

        public bool IsValid(decimal value) => _validatorFunc(value);
    }
}

public class AmountUnit : SmartEnum<AmountUnit>
{
    private AmountUnit(string name, MeasurementType measurement, int value, decimal standardUnitFactor = 1m)
        : base(name, value)
    {
        Measurement = measurement;
        StandardUnitFactor = standardUnitFactor;
    }

    public MeasurementType Measurement { get; }

    public decimal StandardUnitFactor { get; }

    public static AmountUnit Pieces { get; } = new AmountUnit(nameof(Pieces), MeasurementType.Count, 0);
}

public class MeasurementType : SmartEnum<MeasurementType>
{
    private MeasurementType(string name, int value)
        : base(name, value)
    {
    }

    public static MeasurementType Count { get; } = new MeasurementType(nameof(Count), 0);
    public static MeasurementType Length { get; } = new MeasurementType(nameof(Length), 1);
    public static MeasurementType Area { get; } = new MeasurementType(nameof(Area), 2);
    public static MeasurementType Volume { get; } = new MeasurementType(nameof(Volume), 3);
    public static MeasurementType Weight { get; } = new MeasurementType(nameof(Weight), 4);
    public static MeasurementType Temperature { get; } = new MeasurementType(nameof(Temperature), 5);
}
