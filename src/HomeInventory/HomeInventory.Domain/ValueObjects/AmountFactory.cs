using FluentResults;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

internal sealed class AmountFactory : ValueObjectFactory<Amount>, IAmountFactory
{
    private static readonly IReadOnlyDictionary<AmountUnit, Validator> _validators = new Validator[]
    {
        new(AmountUnit.Piece, x => x >= 0m && decimal.Truncate(x) == x),
        new(AmountUnit.Gallon, x => x >= 0m),
        new(AmountUnit.CubicMeter, x => x >= 0m),
    }.ToDictionary(x => x.Unit);

    public Result<Amount> Create(decimal value, AmountUnit unit) => Create(value, GetValidator(unit));

    private static Result<Validator> GetValidator(AmountUnit unit) =>
        _validators.TryGetValue(unit, out var validator) ? validator : Result.Fail<Validator>(new ValidatorNotFoundError(unit));

    private Result<Amount> Create(decimal value, Result<Validator> validationResult) =>
        validationResult.Bind(v => Result.FailIf(!v.IsValid(value), GetValidationError((value, v.Unit))).Bind(() => Result.Ok(new Amount(value, v.Unit))));

    private class Validator
    {
        private readonly Func<decimal, bool> _validatorFunc;

        public Validator(AmountUnit unit, Func<decimal, bool> validatorFunc) => (Unit, _validatorFunc) = (unit, validatorFunc);

        public AmountUnit Unit { get; }

        public bool IsValid(decimal value) => _validatorFunc(value);
    }
}
