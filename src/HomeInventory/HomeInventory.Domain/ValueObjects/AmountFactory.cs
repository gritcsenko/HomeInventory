using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Domain.ValueObjects;

internal sealed class AmountFactory : ValueObjectFactory<Amount>, IAmountFactory
{
    private static readonly IReadOnlyDictionary<AmountUnit, Validator> _validators = new Validator[]
    {
        new(AmountUnit.Piece, x => x >= 0m && decimal.Truncate(x) == x),
        new(AmountUnit.Gallon, x => x >= 0m),
        new(AmountUnit.CubicMeter, x => x >= 0m),
    }.ToDictionary(x => x.Unit);

    public OneOf<Amount, IError> Create(decimal value, AmountUnit unit) => Create(value, GetValidator(unit));

    private static OneOf<Validator, IError> GetValidator(AmountUnit unit) =>
        _validators.TryGetValue(unit, out var validator) ? validator : new ValidatorNotFoundError(unit);

    private OneOf<Amount, IError> Create(decimal value, OneOf<Validator, IError> validationResult)
    {
        return validationResult.Match<OneOf<Amount, IError>>(v =>
        {
            if (v.IsValid(value))
            {
                return new Amount(value, v.Unit);
            }
            else
            {
                return GetValidationError((value, v.Unit));
            }
        }, e => OneOf<Amount, IError>.FromT1(e));
    }

    private class Validator
    {
        private readonly Func<decimal, bool> _validatorFunc;

        public Validator(AmountUnit unit, Func<decimal, bool> validatorFunc) => (Unit, _validatorFunc) = (unit, validatorFunc);

        public AmountUnit Unit { get; }

        public bool IsValid(decimal value) => _validatorFunc(value);
    }
}
