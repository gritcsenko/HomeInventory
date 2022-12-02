using FluentResults;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives;
using OneOf;

namespace HomeInventory.Domain.ValueObjects;

internal sealed class AmountFactory : ValueObjectFactory<Amount>, IAmountFactory
{
    private static readonly IReadOnlyDictionary<AmountUnit, Func<decimal, bool>> Validators = new Dictionary<AmountUnit, Func<decimal, bool>>
    {
        [AmountUnit.Piece] = x => x >= 0m && decimal.Truncate(x) == x,
        [AmountUnit.Gallon] = x => x >= 0m,
        [AmountUnit.CubicMeter] = x => x >= 0m,
    };

    public OneOf<Amount, IError> Create(decimal value, AmountUnit unit)
    {
        var validationResult = Validators.GetValueOrFail(unit, x => new ValidatorNotFoundError(x));
        if (validationResult.HasError<ValidatorNotFoundError>(out var errors))
        {
            return CompositeError.Create(errors);
        }

        var validateFunc = validationResult.Value;
        return TryCreate(() => validateFunc(value), () => CreateValidationError((value, unit)), () => new Amount(value, unit));
    }
}
