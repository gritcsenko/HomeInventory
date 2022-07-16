using Ardalis.SmartEnum;
using ErrorOr;
using System.Diagnostics.CodeAnalysis;

namespace HomeInventory.Domain.ValueObjects;

public class Amount<TValue> : ValueObject<Amount<TValue>, (TValue, AmountUnit)>
{
    internal Amount((TValue, AmountUnit) value, IEqualityComparer<(TValue, AmountUnit)> equalityComparer)
        : base(value, equalityComparer)
    {
    }
}

public interface IAmountFactory
{
    ErrorOr<Amount<int>> Pieces(int value);
}

public class AmountFactory : IAmountFactory
{
    private readonly IAmountFactory<int> _piecesFactory = new AmountFactory<int>(AmountUnit.Pieces.CreateValidator((int x) => x > 0));

    public ErrorOr<Amount<int>> Pieces(int value) => _piecesFactory.Create(value);
}

public abstract class AmountValueValidator<TValue> : IValueValidator<(TValue, AmountUnit)>, IValueValidator<TValue>
{
    protected AmountValueValidator(AmountUnit unit) => Unit = unit;

    public AmountUnit Unit { get; }

    public abstract bool IsValid(TValue value);

    public bool IsValid((TValue, AmountUnit) value) => Unit.Equals(value.Item2) && IsValid(value.Item1);
}

public interface IAmountFactory<TValue>
    where TValue : notnull
{
    ErrorOr<Amount<TValue>> Create(TValue value);
}

public class AmountFactory<TValue> : ValueObjectFactory<Amount<TValue>, (TValue, AmountUnit)>, IAmountFactory<TValue>
    where TValue : notnull
{
    private readonly AmountValueValidator<TValue> _validator;

    public AmountFactory(AmountValueValidator<TValue> validator)
        : base(validator, AmountValueEqualityComparer.Instance) => _validator = validator;

    public ErrorOr<Amount<TValue>> Create(TValue value) => _validator.IsValid(value) ? CreateObject((value, _validator.Unit)) : GetValidationError();

    protected override Amount<TValue> CreateObject((TValue, AmountUnit) value) => new(value, EqualityComparer);

    private class AmountValueEqualityComparer : IEqualityComparer<(TValue, AmountUnit)>
    {
        public static AmountValueEqualityComparer Instance { get; } = new AmountValueEqualityComparer();

        public bool Equals((TValue, AmountUnit) x, (TValue, AmountUnit) y) => x.Item1.Equals(y.Item1) && x.Item2.Equals(y.Item2);

        public int GetHashCode([DisallowNull] (TValue, AmountUnit) obj) => HashCode.Combine(obj.Item1, obj.Item2);
    }
}

public class AmountUnit : SmartEnum<AmountUnit>
{
    private AmountUnit(string name, int value)
        : base(name, value)
    {
    }

    public static AmountUnit Pieces { get; } = new AmountUnit(nameof(Pieces), 0);

    internal AmountValueValidator<TValue> CreateValidator<TValue>(Func<TValue, bool> validatorFunc)
        => new FuncAmountValueValidator<TValue>(this, validatorFunc);

    private class FuncAmountValueValidator<TValue> : AmountValueValidator<TValue>
    {
        private readonly Func<TValue, bool> _validatorFunc;

        public FuncAmountValueValidator(AmountUnit unit, Func<TValue, bool> validatorFunc)
            : base(unit) => _validatorFunc = validatorFunc;

        public override bool IsValid(TValue value) => _validatorFunc(value);
    }
}

