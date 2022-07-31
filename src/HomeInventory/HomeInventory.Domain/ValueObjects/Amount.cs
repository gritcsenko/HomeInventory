﻿using ErrorOr;

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

    ErrorOr<Amount> Gallons(decimal value);
}

public class AmountFactory : ValueObjectFactory<Amount>, IAmountFactory
{
    private static readonly Validator _pieceValidator = new(AmountUnit.Piece, x => x >= 0m);
    private static readonly Validator _gallonValidator = new(AmountUnit.Gallon, x => x >= 0m);

    public ErrorOr<Amount> Pieces(int value) => Create(value, _pieceValidator);

    public ErrorOr<Amount> Gallons(decimal value) => Create(value, _gallonValidator);

    private ErrorOr<Amount> Create(decimal value, Validator validator) => validator.IsValid(value) ? new Amount(value, validator.Unit) : GetValidationError();

    private class Validator
    {
        private readonly Func<decimal, bool> _validatorFunc;

        public Validator(AmountUnit unit, Func<decimal, bool> validatorFunc) => (Unit, _validatorFunc) = (unit, validatorFunc);

        public AmountUnit Unit { get; }

        public bool IsValid(decimal value) => _validatorFunc(value);
    }
}
