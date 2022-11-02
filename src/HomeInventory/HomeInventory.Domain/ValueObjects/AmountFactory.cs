using FluentResults;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Extensions;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

internal sealed class AmountFactory : ValueObjectFactory<Amount>, IAmountFactory
{
    private static readonly IReadOnlyDictionary<AmountUnit, Func<decimal, bool>> Validators = new Dictionary<AmountUnit, Func<decimal, bool>>
    {
        [AmountUnit.Piece] = x => x >= 0m && decimal.Truncate(x) == x,
        [AmountUnit.Gallon] = x => x >= 0m,
        [AmountUnit.CubicMeter] = x => x >= 0m,
    };

    public Result<Amount> Create(decimal value, AmountUnit unit) =>
        Validators.GetValueOrFail(unit, x => new ValidatorNotFoundError(x))
            .Bind(v => TryCreate(() => v(value), () => CreateValidationError((value, unit)), () => new Amount(value, unit)));
}
