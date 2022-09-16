using ErrorOr;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

public sealed class AmountFactory : ValueObjectFactory<Amount>, IAmountFactory
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
